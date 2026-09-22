using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace ClientLifetime;

public static class Workload
{
    static readonly HashSet<int> Ports = new();
    internal static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/downstream", (HttpContext ctx) => { lock (Ports) Ports.Add(ctx.Connection.RemotePort); return "pong"; });
        app.MapGet("/api/{id:int}", async (int id, IHttpClientFactory factory) =>
        {
            var client = factory.CreateClient();                                   // pooled, lifetime-managed handlers
            return await client.GetStringAsync(Workload.Rig.BaseUrl + "/downstream");
        });
    },
    b => b.Services.AddHttpClient());

    public static void Reset() { lock (Ports) Ports.Clear(); }   // scaffolding

    public static long Run() { lock (Ports) Ports.Clear(); var r = Rig.Drive(users: 16, total: 400, i => "/api/" + i); lock (Ports) Lab.Report("connections", Ports.Count); return r; }
}
