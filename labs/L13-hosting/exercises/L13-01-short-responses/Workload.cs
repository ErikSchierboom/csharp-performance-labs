using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace ShortResponses;

public static class Workload
{
    private static readonly HashSet<int> Ports = new();

    private static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.Use(async (ctx, next) =>
        {
            ctx.Response.Headers.Connection = "close"; // pasted from a proxy configuration snippet
            await next();
        });
        app.MapGet("/ping", (HttpContext ctx) => { lock (Ports) Ports.Add(ctx.Connection.RemotePort); return "pong"; });
    });

    public static void Reset() { lock (Ports) Ports.Clear(); }  // scaffolding

    public static long Run() { lock (Ports) Ports.Clear(); var r = Rig.Drive(users: 16, total: 1200, i => "/ping"); lock (Ports) Lab.Report(Metrics.Connections, Ports.Count); return r; }
}
