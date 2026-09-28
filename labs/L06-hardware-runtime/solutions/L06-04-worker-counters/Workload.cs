namespace WorkerCounters;

public static class Workload
{
    private const int Stride = 8; // 8 longs = 64 bytes: one counter per cache line

    public static long Run()
    {
        var counters = new long[8 * Stride];
        var threads = new Thread[8];
        for (var t = 0; t < 8; t++)
        {
            var id = t * Stride;
            threads[t] = new Thread(() =>
            {
                for (var i = 0; i < 5_000_000; i++) Interlocked.Increment(ref counters[id]);
            });
            threads[t].Start();
        }
        foreach (var th in threads) th.Join();
        long total = 0;
        for (var t = 0; t < 8; t++) total += counters[t * Stride];
        return total;
    }
}
