using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace Cancellation;

public static class Workload
{
    static int _stepsAfterAbort;

    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/slow/{id:int}", async (int id, HttpContext ctx) =>
    {
        for (int step = 0; step < 20; step++)                                  // 20 steps of 10 ms "work"
        {
            await Task.Delay(10, ctx.RequestAborted);              // stops at the next await when the client disconnects
            if (ctx.RequestAborted.IsCancellationRequested) Interlocked.Increment(ref _stepsAfterAbort);
        }
        return "done";
    }));

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); _stepsAfterAbort = 0; }   // scaffolding

    public static long Run() { var r = Rig.DriveAbandon(users: 20, total: 100, i => "/slow/" + i, abandonAfterMs: 30);   // every client gives up after 30 ms
        Thread.Sleep(400);                                                                       // let the server drain
        Lab.Report("stepsAfterAbort", Volatile.Read(ref _stepsAfterAbort));
        return r; }
}
