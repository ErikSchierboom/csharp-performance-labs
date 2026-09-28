using System.Diagnostics;
using PerfLab.Harness;

namespace CheckoutService;

/// <summary>The product database. Every lookup is a 3 ms round trip.</summary>
public sealed class ProductRepository
{
    public async Task<int> GetPriceAsync(int sku) { await Task.Delay(3); return 100 + sku % 50; }
}

public sealed class PriceCache(ProductRepository repo)
{
    // One Lazy<Task> per SKU: concurrent requests for the same SKU share a single load (no stampede),
    // nothing blocks while loading, and no lock is held.
    readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<int>>> _map = new();

    public Task<int> GetAsync(int sku) => _map.GetOrAdd(sku, s => new Lazy<Task<int>>(() => repo.GetPriceAsync(s))).Value;
}

public sealed class CheckoutHandler(PriceCache cache)
{
    public async Task<string> HandleAsync(int requestId)
    {
        // Look up all 20 prices concurrently (bounded by the number of line items), then build the receipt once.
        var skus = new int[20]; var qtys = new int[20];
        for (int i = 0; i < 20; i++) { skus[i] = (requestId * 7 + i * 13) % 120; qtys[i] = 1 + (requestId + i) % 5; }
        var prices = await Task.WhenAll(skus.Select(cache.GetAsync));

        long total = 0;
        var sb = new System.Text.StringBuilder(512);
        for (int i = 0; i < 20; i++) { total += (long)prices[i] * qtys[i]; sb.Append(skus[i]).Append(',').Append(qtys[i]).Append(',').Append(prices[i]).Append('\n'); }
        return sb.Append(total).ToString();
    }
}

public static class Workload
{
    private static PriceCache _cache = new(new ProductRepository());
    public static void Reset() { _cache = new PriceCache(new ProductRepository()); ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run()
    {
        var handler = new CheckoutHandler(_cache);
        var t0 = Stopwatch.GetTimestamp();
        var tasks = Enumerable.Range(0, 60).Select(async id =>
        {
            var receipt = await handler.HandleAsync(id);
            Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
            return receipt;
        }).ToArray();
        Task.WaitAll(tasks);
        long h = 0;
        foreach (var t in tasks) { h = h * 31 + t.Result.Length; foreach (char ch in t.Result) h += ch; }
        return h;
    }
}
