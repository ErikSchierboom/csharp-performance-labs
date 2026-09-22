using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace BoundedCache;

public static class Workload
{
    static Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions { SizeLimit = 500 });
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/search/{q:int}", (int q) =>
    {
        return _cache.GetOrCreate(q, e =>
        {
            e.SetSize(1);                                                                          // every entry counts as 1 unit against SizeLimit
            e.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return new string('x', 4_000);
        }) ?? "";
    }));

    public static void Reset() { _cache = new(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions { SizeLimit = 500 }); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 3000, i => "/search/" + i); }
}
