using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace SearchResults;

public static class Workload
{
    private static MemoryCache _cache = new(new MemoryCacheOptions { SizeLimit = 500 });

    private static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/search/{q:int}", (int q) =>
    {
        return _cache.GetOrCreate(q, e =>
        {
            e.SetSize(1); // every entry counts as 1 unit against SizeLimit
            e.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return new string('x', 4_000);
        }) ?? "";
    }));

    public static void Reset() { _cache.Clear(); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 3000, i => "/search/" + i); }
}
