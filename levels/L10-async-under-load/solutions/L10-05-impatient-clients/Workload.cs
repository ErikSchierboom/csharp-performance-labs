using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace ImpatientClients;

public static class Workload
{
    private static int _stepsAfterAbort, _inFlight;
    private static readonly SemaphoreSlim Capacity = new(4); // stands in for a DB pool / downstream limit: 4 requests at a time

    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/slow/{id:int}", async (int id, HttpContext ctx) =>
    {
        var ct = ctx.RequestAborted;
        Lab.Report("peakInFlight", Interlocked.Increment(ref _inFlight)); // requests inside the handler, queued or working
        try
        {
            await Capacity.WaitAsync(ct); // an abandoned request leaves the queue instead of taking a slot
            try
            {
                for (var step = 0; step < 20; step++) // 20 steps of 10 ms "work"
                {
                    await Task.Delay(10, ct); // stops at the next await when the client disconnects, then the slot is released
                    if (ct.IsCancellationRequested) Interlocked.Increment(ref _stepsAfterAbort);
                }
            }
            finally { Capacity.Release(); }
            return $"done {id}";
        }
        finally { Interlocked.Decrement(ref _inFlight); }
    }));

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); _stepsAfterAbort = 0; }   // scaffolding

    // 100 requests from 20 users. Every 4th client is patient and waits for its answer; the rest give up after 30 ms.
    public static long Run() { var r = Rig.DriveAbandon(users: 20, total: 100, i => "/slow/" + i, i => i % 4 == 0 ? 0 : 30);
        while (Volatile.Read(ref _inFlight) > 0) Thread.Sleep(10); // wait until the server is idle again
        Lab.Report("stepsAfterAbort", Volatile.Read(ref _stepsAfterAbort));
        return r; }
}
