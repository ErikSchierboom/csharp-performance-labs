using PerfLab.Harness;

namespace FanOut;

/// <summary>A downstream service whose latency grows sharply when too many calls are in flight.</summary>
public sealed class Downstream
{
    int _inflight, _peak;
    public int Peak => _peak;

    public async Task<int> CallAsync(int x)
    {
        int n = Interlocked.Increment(ref _inflight);
        int peak; while (n > (peak = _peak)) Interlocked.CompareExchange(ref _peak, n, peak);
        try
        {
            await Task.Delay(5 + n * n / 2_000);
            return x * 2;
        }
        finally { Interlocked.Decrement(ref _inflight); }
    }
}

public static class Workload
{
    public static long Run()
    {
        var downstream = new Downstream();
        var items = Enumerable.Range(0, 1_000).ToArray();
        var results = new int[items.Length];

        // Bounded concurrency: at most 50 calls in flight, which is what the downstream can comfortably serve.
        Parallel.ForEachAsync(items, new ParallelOptions { MaxDegreeOfParallelism = 50 },
            async (i, ct) => results[i] = await downstream.CallAsync(i)).GetAwaiter().GetResult();

        Lab.Report("peakInflight", downstream.Peak);
        return results.Sum(r => (long)r);
    }
}
