using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace PopularItems;

public static class Workload
{
    static int _loads;
    static Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
    static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Lazy<Task<string>>> _map = new();
    static async Task<string> LoadAsync(int id) { Interlocked.Increment(ref _loads); await Task.Delay(50); return "item-" + id; }        // an expensive load
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/item/{id:int}", async (int id) =>
    {
        // One load per key, shared by every request that arrives while it is in flight.
        return await _map.GetOrAdd(id, k => new Lazy<Task<string>>(() => LoadAsync(k))).Value;
    }));

    public static void Reset() { _cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()); _map.Clear(); }   // scaffolding

    public static long Run() { Interlocked.Exchange(ref _loads, 0); var r = Rig.Drive(users: 40, total: 200, i => "/item/" + i % 5); Lab.Report("loads", Volatile.Read(ref _loads)); return r; }
}
