using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace CatalogService;

public static class Workload
{
    private static readonly HashSet<int> Ports = new();
    private static readonly SemaphoreSlim DownstreamCapacity = new(8);   // the catalogue backend can only serve 8 calls at once
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions { SizeLimit = 80 }); // bounded
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> InFlight = new();
    private static string Payload(int id) => "item-" + id + new string('x', 32_000); // a 32 KB catalogue entry

    // The catalogue backend: a separate service, not the one under test.
    private static readonly WebRig Downstream = WebRig.Start(app => app.MapGet("/downstream/{id:int}", async (HttpContext ctx, int id) =>
    {
        lock (Ports) Ports.Add(ctx.Connection.RemotePort);
        await DownstreamCapacity.WaitAsync();
        try { await Task.Delay(30); } finally { DownstreamCapacity.Release(); }
        return Payload(id);
    }));

    // The service under test: fronts the backend with a cache.
    private static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/product/{id:int}", async (int id, IHttpClientFactory factory) =>
    {
        if (Cache.TryGetValue(id, out string? cached)) return cached!.Length.ToString();
        var lazy = InFlight.GetOrAdd(id, k => new Lazy<Task<string>>(() => factory.CreateClient().GetStringAsync(Downstream.BaseUrl + "/downstream/" + k)));   // single flight
        string value;
        try { value = await lazy.Value; } finally { InFlight.TryRemove(id, out _); }
        Cache.Set(id, value, new MemoryCacheEntryOptions { Size = 1, AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(100) });   // hot items go cold periodically, like a real cache
        return value.Length.ToString();
    }),
    b => b.Services.AddHttpClient());

    public static void Reset() { lock (Ports) Ports.Clear(); Cache.Clear(); InFlight.Clear(); }   // scaffolding

    // Mostly hot products, with a long tail of one-off ids (search-engine crawlers, old links).
    public static long Run()
    {
        lock (Ports) Ports.Clear();
        var r = Rig.Drive(users: 32, total: 1_500, i => "/product/" + (i % 6 == 0 ? 1_000 + i : i % 20));
        lock (Ports) Lab.Report(Metrics.Connections, Ports.Count);
        return r;
    }
}
