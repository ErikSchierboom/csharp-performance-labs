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

    // "We need the answer right here."
    public int Handle(int id) => _repo.GetScoreAsync(id).Result + 1;
}

public static class Workload
{
    public static void Reset() => ThreadPool.SetMinThreads(4, 4);   // test scaffolding

    public static long Run()
    {
        var handler = new RequestHandler();
        long t0 = Stopwatch.GetTimestamp();                          // 200 requests arrive at once

        var tasks = Enumerable.Range(0, 200).Select(i => Task.Run(() =>
        {
            int r = handler.Handle(i);
            Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
            return r;
        })).ToArray();

        Task.WaitAll(tasks);
        return tasks.Sum(t => (long)t.Result);
    }
}
