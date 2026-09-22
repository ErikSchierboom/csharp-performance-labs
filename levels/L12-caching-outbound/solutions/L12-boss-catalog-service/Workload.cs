using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace CatalogService;

public static class Workload
{
    static readonly HashSet<int> Ports = new();
    static MemoryCache _cache = new(new MemoryCacheOptions { SizeLimit = 80 });                 // bounded
    static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> InFlight = new();
    static string Payload(int id) => "item-" + id + new string('x', 32_000);                  // a 32 KB catalogue entry

    internal static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/downstream/{id:int}", async (HttpContext ctx, int id) =>
        {
            lock (Ports) Ports.Add(ctx.Connection.RemotePort);
            await Task.Delay(30);                                                              // the catalogue backend
            return Payload(id);
        });
        app.MapGet("/product/{id:int}", async (int id, IHttpClientFactory factory) =>
        {
            if (_cache.TryGetValue(id, out string? cached)) return cached!.Length.ToString();
            var lazy = InFlight.GetOrAdd(id, k => new Lazy<Task<string>>(() => factory.CreateClient().GetStringAsync(Workload.Rig.BaseUrl + "/downstream/" + k)));   // single flight
            string value;
            try { value = await lazy.Value; } finally { InFlight.TryRemove(id, out _); }
            _cache.Set(id, value, new MemoryCacheEntryOptions { Size = 1, SlidingExpiration = TimeSpan.FromMinutes(5) });
            return value.Length.ToString();
        });
    },
    b => b.Services.AddHttpClient());

    public static void Reset() { lock (Ports) Ports.Clear(); _cache = new(new MemoryCacheOptions { SizeLimit = 80 }); InFlight.Clear(); }   // scaffolding

    // Mostly hot products, with a long tail of one-off ids (search-engine crawlers, old links).
    public static long Run()
    {
        lock (Ports) Ports.Clear();
        var r = Rig.Drive(users: 32, total: 1_500, i => "/product/" + (i % 6 == 0 ? 1_000 + i : i % 60));
        lock (Ports) Lab.Report("connections", Ports.Count);
        return r;
    }
}
