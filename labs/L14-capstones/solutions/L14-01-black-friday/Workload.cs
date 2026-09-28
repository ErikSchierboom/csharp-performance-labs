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
    static readonly SemaphoreSlim Workers = new(20);   // total request-handling capacity: rejecting still costs a worker
    public static int Calls, GiveUps;
    // Fine up to 15 concurrent calls; above that it fails fast - but a fast rejection still ties up one of its 20
    // workers for the duration, so a flood of retries competes with genuine calls for the same limited pool
    // instead of being free.
    public static async Task<bool> TryAsync()
    {
        Interlocked.Increment(ref Calls);
        await Workers.WaitAsync();
        try
        {
            int n = Interlocked.Increment(ref _inflight);
            try { if (n > 15) { await Task.Delay(20); return false; } await Task.Delay(20); return true; }
            finally { Interlocked.Decrement(ref _inflight); }
        }
        finally { Workers.Release(); }
    }
}

public static class Workload
{
    static readonly SemaphoreSlim Pool = new(25);                     // database connections
    static Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
    static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<bool>>> _map = new();

    static readonly SemaphoreSlim Bulkhead = new(12);                      // never send the third party more than it can take

    static async Task<bool> LoadAsync(int id)
    {
        bool ok;
        await Bulkhead.WaitAsync();
        try { ok = await External.TryAsync(); if (!ok) Interlocked.Increment(ref External.GiveUps); }
        finally { Bulkhead.Release(); }
        if (!ok) return false;                                             // nothing to write: the price lookup failed
        await Pool.WaitAsync();                                            // hold a connection only for the write
        try { await Task.Delay(1); } finally { Pool.Release(); }
        return true;
    }
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/product/{id:int}", async (int id, HttpContext ctx) =>
    {
        var lazy = _map.GetOrAdd(id, k => new Lazy<Task<bool>>(() => LoadAsync(k)));      // one load per product, shared
        var ok = await lazy.Value;
        if (!ok)
        {
            _map.TryRemove(id, out _);                                    // don't let a give-up poison the single-flight cache: retry next time
            ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;   // a give-up is a wrong answer to the client, not a silent "ok"
        }
        return "ok";
    }));

    public static void Reset() { _cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()); _map.Clear(); }   // scaffolding

    public static long Run()
    {
        External.Calls = 0; External.GiveUps = 0;
        var r = Rig.Drive(users: 60, total: 600, i => "/product/" + i % 30);
        Lab.Report(Metrics.ExternalCalls, External.Calls); Lab.Report(Metrics.GiveUps, External.GiveUps);
        return r;
    }
}
