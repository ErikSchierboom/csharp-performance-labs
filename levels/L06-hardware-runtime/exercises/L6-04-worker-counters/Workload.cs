namespace WorkerCounters;

public static class Workload
{
    public static long Run()
    {
        var counters = new long[8];                       // one counter per worker
        var threads = new Thread[8];
        for (int t = 0; t < 8; t++)
        {
            int id = t;
            threads[t] = new Thread(() =>
            {
                for (int i = 0; i < 5_000_000; i++) Interlocked.Increment(ref counters[id]);
            });
            threads[t].Start();
        }
        foreach (var th in threads) th.Join();
        return counters.Sum();
    }
}
