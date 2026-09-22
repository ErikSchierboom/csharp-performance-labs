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
            await Task.Delay(5 + n * n / 2_000);        // 5 ms alone; overload makes it much worse
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

        var results = Task.WhenAll(items.Select(downstream.CallAsync)).GetAwaiter().GetResult();   // all at once

        Lab.Report("peakInflight", downstream.Peak);
        return results.Sum(r => (long)r);
    }
}
