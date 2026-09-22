using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace PoolExhaustion;

public sealed class ConnectionPool
{
    readonly SemaphoreSlim _slots = new(8);                                    // 8 connections in the pool
    public async Task<Lease> RentAsync() { await _slots.WaitAsync(); return new Lease(_slots); }
    public sealed class Lease : IDisposable { readonly SemaphoreSlim _s; public Lease(SemaphoreSlim s) => _s = s; public void Dispose() => _s.Release(); }
}
public static class External { public static Task CallAsync() => Task.Delay(30); }        // a slow third-party API

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        var pool = new ConnectionPool();
        app.MapGet("/order/{id:int}", async (int id) =>
        {
            await External.CallAsync();                      // do the slow, non-database work first
            using var conn = await pool.RentAsync();         // hold the connection only for the 1 ms that needs it
            await Task.Delay(1);
            return "ok";
        });
    });

    public static void Reset() {  }   // scaffolding

    public static long Run() { return Rig.Drive(users: 64, total: 640, i => "/order/" + i); }
}
