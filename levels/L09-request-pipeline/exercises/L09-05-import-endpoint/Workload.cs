using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PerfLab.Harness.Web;

namespace ImportEndpoint;

public record PartnerLine(int Id, string Sku, int Qty, decimal UnitPrice, string Description, string Warehouse);
public record PartnerBatch(string BatchId, List<PartnerLine> Lines);
public record ImportLine(int Id, int Qty);
public record ImportBatch(string BatchId, List<ImportLine> Lines);

public static class Workload
{
    // One partner batch: 1,000 lines, ~170 KB of JSON. Built once, not measured.
    private static readonly byte[] Body = JsonSerializer.SerializeToUtf8Bytes(new PartnerBatch("B-2026-09-26",
        Enumerable.Range(0, 1_000).Select(i => new PartnerLine(i, $"SKU-{i:D6}", i % 9 + 1, 1 + i % 250 / 10m,
            $"Stainless steel fastener, type {i % 17}, {10 + i % 40} mm, pack of {5 * (1 + i % 8)}, zinc-plated, DIN {900 + i % 90}",
            i % 3 == 0 ? "Rotterdam" : "Duisburg")).ToList()));

    private static HttpContent MakeContent(int i)
    {
        var c = new ByteArrayContent(Body);
        c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return c;
    }

    private static readonly WebRig Rig = WebRig.Start(app => app.MapPost("/import", async (HttpContext ctx) =>
    {
        using var reader = new StreamReader(ctx.Request.Body);
        var json = await reader.ReadToEndAsync();
        var batch = JsonSerializer.Deserialize<ImportBatch>(json)!;
        return Results.Text(batch.Lines.Sum(l => l.Qty).ToString());
    }));

    public static long Run() => Rig.Drive(users: 24, total: 1200, i => "/import", HttpMethod.Post, MakeContent);
}
