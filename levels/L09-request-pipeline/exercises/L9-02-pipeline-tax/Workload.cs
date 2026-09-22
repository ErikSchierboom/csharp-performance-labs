using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using PerfLab.Harness.Web;

namespace PipelineTax;

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.Use(async (ctx, next) =>
        {
            var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("audit");
            var match = new Regex(@"^/api/(?<area>\w+)/(?<id>\d+)$").Match(ctx.Request.Path.Value ?? "");      // built per request
            string tag = "area=" + match.Groups["area"].Value + ";id=" + match.Groups["id"].Value;
            logger.LogInformation($"Handling request {ctx.Request.Path} ({tag}) from {ctx.Connection.RemoteIpAddress}");   // formatted even if no one listens
            ctx.Response.Headers["X-Trace"] = tag.ToUpperInvariant();
            await next();
        });
        app.MapGet("/api/orders/{id:int}", (int id) => (id * 2).ToString());
    });

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 32, total: 4000, i => "/api/orders/" + i);
}
