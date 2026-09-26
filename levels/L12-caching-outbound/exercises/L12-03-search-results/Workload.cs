using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace SearchResults;

public static class Workload
{
    static Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/search/{q:int}", (int q) =>
    {
        return _cache.GetOrCreate(q, e => new string('x', 4_000)) ?? "";                        // one 8 KB entry per distinct query, forever
    }));

    public static void Reset() { _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 3000, i => "/search/" + i); }
}
