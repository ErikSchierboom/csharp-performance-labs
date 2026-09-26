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
    static MemoryCache _cache = new(new MemoryCacheOptions());
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/doc/{id:int}", (int id) =>
    {
        var rendered = new byte[100_000];                                           // a rendered document: 100 KB
        rendered[id % rendered.Length] = (byte)id;
        _cache.Set(id, rendered);                                                   // "cache the expensive result", forever
        return rendered.Length.ToString();
    }));

    public static void Reset() { _cache = new(new MemoryCacheOptions()); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 16, total: 1200, i => "/doc/" + i); }
}
