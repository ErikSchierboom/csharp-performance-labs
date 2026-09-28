using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace DownstreamCall;

public static class Workload
{
    static readonly HashSet<int> Ports = new();

    // The downstream service: a separate server, not the one under test.
    private static readonly WebRig Downstream = WebRig.Start(app =>
        app.MapGet("/downstream", (HttpContext ctx) => { lock (Ports) Ports.Add(ctx.Connection.RemotePort); return "pong"; }));

    private static readonly WebRig Rig = WebRig.Start(app =>
        app.MapGet("/api/{id:int}", async (int id) =>
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(Downstream.BaseUrl + "/downstream");
        }));

    public static void Reset() { lock (Ports) Ports.Clear(); }   // scaffolding

    public static long Run() { lock (Ports) Ports.Clear(); var r = Rig.Drive(users: 16, total: 400, i => "/api/" + i); lock (Ports) Lab.Report(Metrics.Connections, Ports.Count); return r; }
}
