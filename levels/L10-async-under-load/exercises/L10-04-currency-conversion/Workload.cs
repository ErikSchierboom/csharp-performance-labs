using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace CurrencyConversion;

public class RateService
{
    // Stand-in for a remote exchange-rate API: 10 ms per call.
    public async Task<decimal> FetchAsync(string currency) { await Task.Delay(10); return currency.Length * 1.25m; }
}

public static class Workload
{
    private static readonly WebRig Rig = WebRig.Start(app =>
    {
        var gate = new SemaphoreSlim(1, 1);
        app.MapGet("/convert/{cur}", async (RateService svc, string cur) =>
        {
            await gate.WaitAsync();
            try { return (100 * await svc.FetchAsync(cur)).ToString("F2"); }
            finally { gate.Release(); }
        });
    }, b => b.Services.AddSingleton<RateService>());

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 50, total: 400, i => "/convert/" + new[] { "EUR", "USD", "GBP", "JPY" }[i % 4]); }
}
