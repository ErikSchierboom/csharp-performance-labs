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
    private static int _inflight;
    static readonly SemaphoreSlim Workers = new(20);   // total request-handling capacity: rejecting still costs a worker
    public static int Calls, GiveUps;
    public static async Task<bool> TryAsync()
    {
        Interlocked.Increment(ref Calls);
        await Workers.WaitAsync();
        try
        {
            var n = Interlocked.Increment(ref _inflight);
            try { if (n > 15) { await Task.Delay(20); return false; } await Task.Delay(20); return true; }
            finally { Interlocked.Decrement(ref _inflight); }
        }
        finally { Workers.Release(); }
    }
}

public static class Workload
{
    private static readonly SemaphoreSlim Pool = new(25); // database connections
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
    static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> _map = new();
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/product/{id:int}", async (int id, HttpContext ctx) =>
    {
        var ok = await Cache.GetOrCreateAsync(id, async e =>
        {
            await Pool.WaitAsync();
            try
            {
                for (int attempt = 0; attempt < 3; attempt++)
                    if (await External.TryAsync()) return true;
                Interlocked.Increment(ref External.GiveUps);
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(1);   // don't let a give-up poison the cache forever: retry next time
                return false;
            }
            finally { Pool.Release(); }
        });
        if (!ok) ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;   // a give-up is a wrong answer to the client, not a silent "ok"
        return "ok";
    }));

    public static void Reset() { Cache.Clear(); _map.Clear(); }   // scaffolding

    public static long Run()
    {
        External.Calls = 0;
        External.GiveUps = 0;
        var r = Rig.Drive(users: 60, total: 600, i => "/product/" + i % 30);   // drives load and records latencies
        Lab.Report(Metrics.ExternalCalls, External.Calls);
        Lab.Report(Metrics.GiveUps, External.GiveUps);
        return r;
    }
}
