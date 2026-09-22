using System.Diagnostics;
using PerfLab.Harness;

namespace CheckoutService;

/// <summary>The product database. Every lookup is a 3 ms round trip.</summary>
public sealed class ProductRepository
{
    public async Task<int> GetPriceAsync(int sku) { await Task.Delay(3); return 100 + sku % 50; }
}

public sealed class PriceCache
{
    readonly object _gate = new();
    readonly Dictionary<int, int> _map = new();
    readonly ProductRepository _repo;
    public PriceCache(ProductRepository repo) => _repo = repo;

    public int Get(int sku)
    {
        lock (_gate)
        {
            if (_map.TryGetValue(sku, out var p)) return p;
            p = _repo.GetPriceAsync(sku).Result;                 // load on a miss
            _map[sku] = p;
            return p;
        }
    }
}

public sealed class CheckoutHandler
{
    readonly PriceCache _cache;
    public CheckoutHandler(PriceCache cache) => _cache = cache;

    public string Handle(int requestId)
    {
        long total = 0;
        var receipt = "";
        for (int i = 0; i < 20; i++)                              // 20 line items per checkout
        {
            int sku = (requestId * 7 + i * 13) % 120;
            int qty = 1 + (requestId + i) % 5;
            int price = _cache.Get(sku);
            total += (long)price * qty;
            receipt += $"{sku},{qty},{price}\n";
        }
        return receipt + total;
    }
}

public static class Workload
{
    static PriceCache _cache = new(new ProductRepository());
    public static void Reset() { _cache = new PriceCache(new ProductRepository()); ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run()
    {
        var handler = new CheckoutHandler(_cache);
        long t0 = Stopwatch.GetTimestamp();                                   // 60 checkout requests arrive together
        var tasks = Enumerable.Range(0, 60).Select(id => Task.Run(() =>
        {
            var receipt = handler.Handle(id);
            Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
            return receipt;
        })).ToArray();
        Task.WaitAll(tasks);
        long h = 0;
        foreach (var t in tasks) { h = h * 31 + t.Result.Length; foreach (char ch in t.Result) h += ch; }
        return h;
    }
}
