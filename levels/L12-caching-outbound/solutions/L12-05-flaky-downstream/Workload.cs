using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace FlakyDownstream;

public static class Downstream
{
    static int _inflight;
    public static int Calls, GiveUps;
    // Works fine up to 20 concurrent calls; above that it fails fast (as an overloaded service does).
    public static async Task<bool> TryCallAsync()
    {
        Interlocked.Increment(ref Calls);
        int n = Interlocked.Increment(ref _inflight);
        try { if (n > 20) { await Task.Delay(2); return false; } await Task.Delay(30); return true; }
        finally { Interlocked.Decrement(ref _inflight); }
    }
}

public static class Workload
{
    static readonly SemaphoreSlim Gate = new(15);
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/quote/{id:int}", async (int id) =>
    {
        await Gate.WaitAsync();                                             // a bulkhead: never send the downstream more than it can take
        try { if (await Downstream.TryCallAsync()) return "ok"; }
        finally { Gate.Release(); }
        Interlocked.Increment(ref Downstream.GiveUps);
        return "na";
    }));

    public static void Reset() {  }   // scaffolding

    public static long Run() { Downstream.Calls = 0; Downstream.GiveUps = 0; var r = Rig.Drive(users: 80, total: 400, i => "/quote/" + i); Lab.Report(Metrics.DownstreamCalls, Downstream.Calls); Lab.Report(Metrics.GiveUps, Downstream.GiveUps); return r; }
}
