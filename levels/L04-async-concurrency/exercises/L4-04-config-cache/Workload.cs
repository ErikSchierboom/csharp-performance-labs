using System.Collections.Concurrent;
using System.Diagnostics;
using PerfLab.Harness;

namespace ConfigCache;

public static class Workload
{
    static int _calls;

    // Loads one config section: ~5 ms of CPU-bound work (parsing, validation...).
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
            var cache = new ConcurrentDictionary<int, int>();      // a fresh cache; 8 threads all want the same section
            Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, _ =>
            {
                int v = cache.GetOrAdd(round, LoadSection);
                Interlocked.Add(ref total, v);
            });
        }
        Lab.Report("factoryCalls", _calls);
        return total;
    }
}
