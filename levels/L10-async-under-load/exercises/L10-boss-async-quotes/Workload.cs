using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace AsyncQuotes;

public sealed class Repository
{
    // A "database driver": calls complete on the driver's own thread, not on the thread pool.
    static readonly System.Collections.Concurrent.BlockingCollection<(long Due, TaskCompletionSource<int> Tcs, int Value)> Queue = new();
    static Repository() => new Thread(() =>
    {
        foreach (var (due, tcs, value) in Queue.GetConsumingEnumerable())
        {
            long wait = due - Environment.TickCount64;
            if (wait > 0) Thread.Sleep((int)wait);
            tcs.TrySetResult(value);
        }
    }) { IsBackground = true }.Start();

    public Task<int> GetScoreAsync(int id)
    {
        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Queue.Add((Environment.TickCount64 + 20, tcs, id * 3 + 1));
        return tcs.Task;
    }
}
public sealed class Downstream
{
    int _inflight, _peak;
    public int Peak => _peak;
    public async Task<int> CallAsync(int x)
    {
        int n = Interlocked.Increment(ref _inflight);
        int peak; while (n > (peak = _peak)) Interlocked.CompareExchange(ref _peak, n, peak);
        try { await Task.Delay(5 + n * n / 4_000); return x + 1; }
        finally { Interlocked.Decrement(ref _inflight); }
    }
}

public static class Workload
{
    static readonly Downstream Down = new();
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        var repo = new Repository();
        app.MapGet("/quote/{id:int}", async (int id) =>
        {
            Thread.Sleep(3);                                                   // a synchronous lookup inside an async handler
            int score = repo.GetScoreAsync(id).Result;                         // blocks a pool thread on the driver
            var calls = Enumerable.Range(0, 10).Select(i => Down.CallAsync(id * 10 + i));
            var results = await Task.WhenAll(calls);                           // ten calls per request, all at once, across every request
            return (score + results.Sum()).ToString();
        });
    });

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); ThreadPool.SetMaxThreads(32, 32); }   // scaffolding

    public static long Run() { var r = Rig.Drive(users: 100, total: 400, i => "/quote/" + i); Lab.Report("peakInflight", Down.Peak); return r; }
}
