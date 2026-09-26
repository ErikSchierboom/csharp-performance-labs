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

public static class Workload
{
    static readonly byte[] Body = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
        new Basket(Enumerable.Range(0, 1_600).Select(i => new Line(i, "item-" + i, i % 9 + 1)).ToList())));      // ~60 KB

    static HttpContent MakeContent(int i)
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
            var m = new Regex(@"^/(?<area>\w+)$").Match(ctx.Request.Path.Value ?? "");
            logger.LogInformation($"Handling {ctx.Request.Path} area={m.Groups["area"].Value}");
            await next();
        });
        app.MapPost("/checkout", async (HttpContext ctx) =>
        {
            using var provider = new ServiceCollection().AddTransient<TaxTable>().BuildServiceProvider();      // a container per request
            var tax = provider.GetRequiredService<TaxTable>();
            string text = await new StreamReader(ctx.Request.Body).ReadToEndAsync();                          // body -> string
            var basket = JsonSerializer.Deserialize<Basket>(text)!;
            long total = basket.Lines.Sum(l => (long)l.Qty * (100 + tax.Rate(l.Id)));
            ctx.Response.ContentType = "text/plain";
            for (int i = 0; i < 60; i++)
            {
                await ctx.Response.WriteAsync($"line {i}: {total + i}\n");
                await ctx.Response.Body.FlushAsync();                                                          // flush per line
            }
        });
    });

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 24, total: 1000, i => "/checkout", HttpMethod.Post, MakeContent);
}
