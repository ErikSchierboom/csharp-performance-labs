using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness.Web;

namespace StorefrontCheckout;

public record Line(int Id, string Name, int Qty);
public record Basket(List<Line> Lines);
public sealed class TaxTable
{
    readonly Dictionary<int, int> _t;
    public TaxTable() => _t = Enumerable.Range(0, 20_000).ToDictionary(i => i, i => i % 23);     // reference data: expensive to build
    public int Rate(int id) => _t[id % 20_000];
}

static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling {Path} area={Area}")]
    public static partial void Handling(ILogger logger, string path, string area);
}

public static class Workload
{
    private static readonly Regex AreaRegex = new(@"^/(?<area>\w+)$", RegexOptions.Compiled);
    private static readonly byte[] Body = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
        new Basket(Enumerable.Range(0, 1_600).Select(i => new Line(i, "item-" + i, i % 9 + 1)).ToList())));      // ~60 KB

    private static HttpContent MakeContent(int i)
    {
        var c = new ByteArrayContent(Body);
        c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return c;
    }

    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.Use(async (ctx, next) =>
        {
            var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("audit");
            var m = AreaRegex.Match(ctx.Request.Path.Value ?? "");
            Log.Handling(logger, ctx.Request.Path.Value ?? "", m.Groups["area"].Value);
            await next();
        });
        app.MapPost("/checkout", async (HttpContext ctx) =>
        {
            var tax = ctx.RequestServices.GetRequiredService<TaxTable>();
            var basket = (await JsonSerializer.DeserializeAsync<Basket>(ctx.Request.Body))!; // stream, no string
            var total = basket.Lines.Sum(l => (long)l.Qty * (100 + tax.Rate(l.Id)));
            ctx.Response.ContentType = "text/plain";
            var sb = new System.Text.StringBuilder(2_000);
            for (int i = 0; i < 60; i++) sb.Append("line ").Append(i).Append(": ").Append(total + i).Append('\n');
            await ctx.Response.WriteAsync(sb.ToString()); // one write
        });
    },
    b => b.Services.AddSingleton<TaxTable>()); // singleton

    public static long Run() => Rig.Drive(users: 24, total: 1000, i => "/checkout", HttpMethod.Post, MakeContent);
}
