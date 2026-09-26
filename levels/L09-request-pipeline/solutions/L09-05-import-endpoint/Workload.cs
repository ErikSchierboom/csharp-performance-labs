using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PerfLab.Harness.Web;

namespace ImportEndpoint;

public record Line(int Id, string Name, int Qty);
public record Batch(List<Line> Lines);

public static class Workload
{
    static readonly byte[] Body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(
        new Batch(Enumerable.Range(0, 1_600).Select(i => new Line(i, "item-" + i, i % 9 + 1)).ToList())));      // ~60 KB of JSON

    static HttpContent MakeContent(int i)
    {
        var c = new ByteArrayContent(Body);
        c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return c;
    }

    static readonly WebRig Rig = WebRig.Start(app => app.MapPost("/import", async (HttpContext ctx) =>
    {
        // deserialise straight from the request stream: no intermediate string
        var batch = await JsonSerializer.DeserializeAsync<Batch>(ctx.Request.Body);
        return Results.Text(batch!.Lines.Sum(l => l.Qty).ToString());
    }));

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 24, total: 1200, i => "/import", HttpMethod.Post, MakeContent);
}
