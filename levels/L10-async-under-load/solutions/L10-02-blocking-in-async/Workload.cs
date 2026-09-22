using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace BlockingInAsync;

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/lookup/{id:int}", async (int id) =>
    {
        await Task.Delay(15);                  // the asynchronous equivalent: the thread is released while waiting
        return (id * 2).ToString();
    }));

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 200, total: 800, i => "/lookup/" + i); }
}
