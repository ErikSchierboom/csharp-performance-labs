using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace DatabaseEndpoint;

public static class UserRepository
{
    // A "database driver": calls complete on the driver's own thread (like a real I/O completion), not on the thread pool.
    private static readonly System.Collections.Concurrent.BlockingCollection<(long Due, TaskCompletionSource<int> Tcs, int Value)> Queue = new();

    static UserRepository() => new Thread(() =>
    {
        foreach (var (due, tcs, value) in Queue.GetConsumingEnumerable())
        {
            var wait = due - Environment.TickCount64;
            if (wait > 0) Thread.Sleep((int)wait);
            tcs.TrySetResult(value);
        }
    }) { IsBackground = true }.Start();

    public static Task<int> GetScoreAsync(int id)
    {
        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Queue.Add((Environment.TickCount64 + 20, tcs, id * 3 + 1)); // a 20 ms round trip
        return tcs.Task;
    }
}

public static class Workload
{
    private static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/user/{id:int}", (int id) => (UserRepository.GetScoreAsync(id).Result + 1).ToString());
    });

    public static void Reset() { ThreadPool.SetMinThreads(4, 4); ThreadPool.SetMaxThreads(32, 32); }   // scaffolding

    public static long Run() { return Rig.Drive(users: 200, total: 800, i => "/user/" + i); }
}
