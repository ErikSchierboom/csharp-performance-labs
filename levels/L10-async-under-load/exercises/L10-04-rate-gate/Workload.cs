using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace RateGate;

public sealed class RateService
{
    // Stand-in for a remote exchange-rate API: 10 ms per call.
    public async Task<decimal> FetchAsync(string currency) { await Task.Delay(10); return currency.Length * 1.25m; }
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        var svc = new RateService();
        var gate = new SemaphoreSlim(1, 1);                       // "only one refresh at a time"
        app.MapGet("/convert/{cur}", async (string cur) =>
        {
            await gate.WaitAsync();
            try { return (100 * await svc.FetchAsync(cur)).ToString("F2"); }      // every request fetches, one at a time
            finally { gate.Release(); }
        });
    });

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 50, total: 400, i => "/convert/" + (new[] { "EUR", "USD", "GBP", "JPY" })[i % 4]); }
}
