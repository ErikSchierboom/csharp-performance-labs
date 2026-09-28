using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace FlakyDownstream;

public static class Downstream
{
    static int _inflight;
    private static readonly SemaphoreSlim Workers = new(25);   // total request-handling capacity: rejecting still costs a worker
    public static int Calls, GiveUps;
    public static async Task<bool> TryCallAsync()
    {
        Interlocked.Increment(ref Calls);
        await Workers.WaitAsync();
        try
        {
            var n = Interlocked.Increment(ref _inflight);
            try { if (n > 20) { await Task.Delay(30); return false; } await Task.Delay(30); return true; }
            finally { Interlocked.Decrement(ref _inflight); }
        }
        finally { Workers.Release(); }
    }
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/quote/{id:int}", async (int id) =>
    {
        for (var attempt = 0; attempt < 3; attempt++)
            if (await Downstream.TryCallAsync()) return "ok";
        Interlocked.Increment(ref Downstream.GiveUps);
        return "na";
    }));

    public static long Run()
    {
        Downstream.Calls = 0; 
        Downstream.GiveUps = 0; 
        var r = Rig.Drive(users: 80, total: 400, i => "/quote/" + i); 
        Lab.Report(Metrics.DownstreamCalls, Downstream.Calls); 
        Lab.Report(Metrics.GiveUps, Downstream.GiveUps); 
        return r;
    }
}
