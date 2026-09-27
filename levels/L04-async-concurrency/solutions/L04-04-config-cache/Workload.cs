using System.Collections.Concurrent;
using System.Diagnostics;
using PerfLab.Harness;

namespace ConfigCache;

public static class Workload
{
    static int _calls;

    static int LoadSection(int key)
    {
        Interlocked.Increment(ref _calls);
        long acc = key; var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalMilliseconds < 5) acc = acc * 31 + 7;
        return key * key + 1 + (int)(acc & 0);
    }

    public static long Run()
    {
        _calls = 0;
        long total = 0;
        for (int round = 0; round < 20; round++)
        {
            // Cache a Lazy<T>: GetOrAdd may still create several Lazy objects (cheap), but only ONE of them is ever evaluated.
            var cache = new ConcurrentDictionary<int, Lazy<int>>();
            Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, _ =>
            {
                int v = cache.GetOrAdd(round, k => new Lazy<int>(() => LoadSection(k))).Value;
                Interlocked.Add(ref total, v);
            });
        }
        Lab.Report(Metrics.FactoryCalls, _calls);
        return total;
    }
}
