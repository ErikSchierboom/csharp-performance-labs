using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace BlackFriday;

public static class External
{
    static int _inflight;
    public static int Calls, GiveUps;
    // Fine up to 15 concurrent calls; above that it fails fast.
    public static async Task<bool> TryAsync()
    {
        Interlocked.Increment(ref Calls);
        int n = Interlocked.Increment(ref _inflight);
        try { if (n > 15) { await Task.Delay(2); return false; } await Task.Delay(20); return true; }
        finally { Interlocked.Decrement(ref _inflight); }
    }
}

public static class Workload
{
    static readonly SemaphoreSlim Pool = new(25);                     // database connections
    static Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
    static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> _map = new();

    static readonly SemaphoreSlim Bulkhead = new(12);                      // never send the third party more than it can take

    static async Task<string> LoadAsync(int id)
    {
        await Bulkhead.WaitAsync();
        try { if (!await External.TryAsync()) Interlocked.Increment(ref External.GiveUps); }
        finally { Bulkhead.Release(); }
        await Pool.WaitAsync();                                            // hold a connection only for the write
        try { await Task.Delay(1); } finally { Pool.Release(); }
        return "ok";
    }
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/product/{id:int}", async (int id) =>
    {
        await _map.GetOrAdd(id, k => new Lazy<Task<string>>(() => LoadAsync(k))).Value;      // one load per product, shared
        return "ok";
    }));

    public static void Reset() { _cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()); _map.Clear(); }   // scaffolding

    public static long Run() { External.Calls = 0; External.GiveUps = 0; var r = Rig.Drive(users: 60, total: 600, i => "/product/" + i % 30); Lab.Report(Metrics.ExternalCalls, External.Calls); Lab.Report(Metrics.GiveUps, External.GiveUps); return r; }
}
