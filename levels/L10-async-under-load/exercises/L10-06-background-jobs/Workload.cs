using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace BackgroundJobs;

public static class Workload
{
    private static int _running, _peak, _done;

    // A background job: ~5 ms of blocking work (a synchronous report generator, say).
    private static void Job()
    {
        var n = Interlocked.Increment(ref _running);
        int peak; while (n > (peak = Volatile.Read(ref _peak))) Interlocked.CompareExchange(ref _peak, n, peak);
        Thread.Sleep(5);
        Interlocked.Decrement(ref _running);
        Interlocked.Increment(ref _done);
    }

    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapPost("/enqueue/{id:int}", (int id) => { _ = Task.Run(Job); return Results.Accepted(); }); // "do it in the background"
    });

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); _running = _peak = _done = 0; }   // scaffolding

    public static long Run() { var r = Rig.Drive(users: 100, total: 300, i => "/enqueue/" + i, HttpMethod.Post);
        SpinWait.SpinUntil(() => Volatile.Read(ref _done) >= 300, 60_000); // wait for the background jobs to finish
        Lab.Report("peakJobs", Volatile.Read(ref _peak));
        return r; }
}
