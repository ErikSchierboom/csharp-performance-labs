using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace PopularItems;

public static class Workload
{
    private static int _loads;
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> _map = new();
    private static readonly SemaphoreSlim Backend = new(8);   // the real backend can only serve 8 calls at once
    private static async Task<string> LoadAsync(int id)
    {
        await Backend.WaitAsync();
        try { Interlocked.Increment(ref _loads); await Task.Delay(50); return "item-" + id; }   // an expensive load
        finally { Backend.Release(); }
    }

    private static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/item/{id:int}", async (int id) =>
    {
        // One load per key, shared by every request that arrives while it is in flight.
        return await _map.GetOrAdd(id, k => new Lazy<Task<string>>(() => LoadAsync(k))).Value;
    }));

    public static void Reset() { _map.Clear(); }   // scaffolding

    public static long Run() { Interlocked.Exchange(ref _loads, 0); var r = Rig.Drive(users: 40, total: 200, i => "/item/" + i % 5); Lab.Report(Metrics.Loads, Volatile.Read(ref _loads)); return r; }
}
