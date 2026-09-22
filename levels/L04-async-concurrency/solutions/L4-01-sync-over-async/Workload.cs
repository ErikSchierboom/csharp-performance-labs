using System.Diagnostics;
using PerfLab.Harness;

namespace SyncOverAsync;

public sealed class UserRepository
{
    public async Task<int> GetScoreAsync(int id)
    {
        await Task.Delay(20);                 // a database call
        return id * 3 + 1;
    }
}

public sealed class RequestHandler
{
    readonly UserRepository _repo = new();

    // Async all the way: no thread sits blocked while the "database" works.
    public async Task<int> HandleAsync(int id) => await _repo.GetScoreAsync(id) + 1;
}

public static class Workload
{
    public static void Reset() => ThreadPool.SetMinThreads(4, 4);   // test scaffolding

    public static long Run()
    {
        var handler = new RequestHandler();
        long t0 = Stopwatch.GetTimestamp();

        var tasks = Enumerable.Range(0, 200).Select(async i =>
        {
            int r = await handler.HandleAsync(i);
            Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
            return r;
        }).ToArray();

        Task.WaitAll(tasks);                 // one blocking wait at the very edge (the harness); the handlers don't block
        return tasks.Sum(t => (long)t.Result);
    }
}
