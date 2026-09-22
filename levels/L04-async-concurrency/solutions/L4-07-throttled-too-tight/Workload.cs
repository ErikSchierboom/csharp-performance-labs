namespace Throttled;

public static class Workload
{
    public static void Reset() { }

    public static long Run()
    {
        var results = new int[200];
        // The API comfortably serves 32 concurrent calls: use them.
        Parallel.ForEachAsync(Enumerable.Range(0, 200), new ParallelOptions { MaxDegreeOfParallelism = 32 }, async (i, ct) =>
        {
            await Task.Delay(10, ct);
            results[i] = i * 2;
        }).GetAwaiter().GetResult();
        return results.Sum(r => (long)r);
    }
}
