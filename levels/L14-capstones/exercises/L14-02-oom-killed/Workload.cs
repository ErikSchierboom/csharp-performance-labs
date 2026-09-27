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
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    private static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/doc/{id:int}", (int id) =>
    {
        var rendered = new byte[100_000]; // a rendered document: 100 KB
        rendered[id % rendered.Length] = (byte)id;
        Cache.Set(id, rendered);
        return rendered.Length.ToString();
    }));

    public static void Reset() { Cache.Clear(); } // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 1200, i => "/doc/" + i); }
}
