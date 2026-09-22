using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace EndpointFanOut;

/// <summary>A shared downstream service that gets much slower when too many calls are in flight (as most do).</summary>
public sealed class Downstream
{
    int _inflight, _peak;
    public int Peak => _peak;
    public void ResetPeak() => _peak = 0;
    public async Task<int> CallAsync(int x)
    {
        int n = Interlocked.Increment(ref _inflight);
        int peak; while (n > (peak = _peak)) Interlocked.CompareExchange(ref _peak, n, peak);
        try { await Task.Delay(5 + n * n / 2_000); return x + 1; }
        finally { Interlocked.Decrement(ref _inflight); }
    }
}

public static class Workload
{
    static readonly Downstream Down = new();
    static readonly SemaphoreSlim Gate = new(40);          // what the downstream can comfortably serve at once

    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/aggregate/{id:int}", async (int id) =>
        {
            var results = new int[30];
            await Task.WhenAll(Enumerable.Range(0, 30).Select(async i =>
            {
                await Gate.WaitAsync();                                  // a limit shared by ALL requests
                try { results[i] = await Down.CallAsync(id * 100 + i); }
                finally { Gate.Release(); }
            }));
            return results.Sum().ToString();
        });
    });

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run() { var r = Rig.Drive(users: 16, total: 160, i => "/aggregate/" + i); Lab.Report("peakInflight", Down.Peak); return r; }
}
