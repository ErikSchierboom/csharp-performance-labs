using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness.Web;

namespace ReportEndpoint;

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/report", async (HttpContext ctx) =>
    {
        ctx.Response.ContentType = "text/plain";
        for (int i = 0; i < 300; i++)
        {
            await ctx.Response.WriteAsync($"line {i}: the quick brown fox\n");
            await ctx.Response.Body.FlushAsync();                                  // "stream it as we go"
        }
    }));

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 32, total: 1500, i => "/report");
}
