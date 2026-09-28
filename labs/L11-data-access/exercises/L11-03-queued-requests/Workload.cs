using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace QueuedRequests;

public sealed class ConnectionPool
{
    private readonly SemaphoreSlim _slots = new(8);
    public async Task<Lease> RentAsync() { await _slots.WaitAsync(); return new Lease(_slots); }

    public sealed class Lease(SemaphoreSlim s) : IDisposable { public void Dispose() => s.Release(); }
}
public static class External { public static Task CallAsync() => Task.Delay(30); } // a slow third-party API

public static class Workload
{
    private static readonly WebRig Rig = WebRig.Start(app =>
    {
        var pool = new ConnectionPool();
        app.MapGet("/order/{id:int}", async (int id) =>
        {
            using var conn = await pool.RentAsync();
            await External.CallAsync();
            await Task.Delay(1); // the actual query (1 ms)
            return "ok";
        });
    });

    public static long Run() { return Rig.Drive(users: 64, total: 640, i => "/order/" + i); }
}
