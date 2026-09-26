using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using PerfLab.Harness.Web;

namespace AuditMiddleware;

static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling request {Path} ({Tag}) from {Remote}")]
    public static partial void Handling(ILogger logger, string path, string tag, System.Net.IPAddress? remote);
}


public static class Workload
{
    internal static readonly Regex AuditRegex = new(@"^/api/(?<area>\w+)/(?<id>\d+)$", RegexOptions.Compiled);

    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.Use(async (ctx, next) =>
        {
            var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("audit");
            var match = Workload.AuditRegex.Match(ctx.Request.Path.Value ?? "");
            string tag = string.Concat("area=", match.Groups["area"].Value, ";id=", match.Groups["id"].Value);
            Log.Handling(logger, ctx.Request.Path.Value ?? "", tag, ctx.Connection.RemoteIpAddress);         // no work when the level is off
            ctx.Response.Headers["X-Trace"] = tag.ToUpperInvariant();
            await next();
        });
        app.MapGet("/api/orders/{id:int}", (int id) => (id * 2).ToString());
    });

    public static long Run() => Rig.Drive(users: 32, total: 4000, i => "/api/orders/" + i);
}
