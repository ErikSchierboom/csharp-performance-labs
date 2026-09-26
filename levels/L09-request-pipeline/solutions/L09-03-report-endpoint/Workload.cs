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
        var sb = new System.Text.StringBuilder(300 * 32);
        for (int i = 0; i < 300; i++) sb.Append("line ").Append(i).Append(": the quick brown fox\n");
        await ctx.Response.WriteAsync(sb.ToString());                              // one write; the server sends it in one go
    }));

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 32, total: 1500, i => "/report");
}
