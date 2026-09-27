using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Extensions.Caching.Memory;

namespace OomKilled;

public static class Workload
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions { SizeLimit = 500 });

    private static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/doc/{id:int}", (int id) =>
    {
        var rendered = System.Buffers.ArrayPool<byte>.Shared.Rent(100_000); // scratch buffer from the pool: no per-request LOH garbage
        try
        {
            Array.Clear(rendered, 0, 100_000);
            rendered[id % 100_000] = (byte)id;
            Cache.GetOrCreate(id, e => { e.SetSize(1); e.SetSlidingExpiration(TimeSpan.FromSeconds(30)); return rendered[id % 100_000]; });   // cache the small answer, bounded
            return 100_000.ToString();
        }
        finally { System.Buffers.ArrayPool<byte>.Shared.Return(rendered); }
    }));

    public static void Reset() { Cache.Clear(); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 1200, i => "/doc/" + i); }
}
