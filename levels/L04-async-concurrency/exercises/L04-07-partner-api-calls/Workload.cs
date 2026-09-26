namespace PartnerApiCalls;

public static class Workload
{
    public static void Reset() { }

    public static long Run()
    {
        var results = new int[200];
        // "Be nice to the API": at most ONE call in flight.
        Parallel.ForEachAsync(Enumerable.Range(0, 200), new ParallelOptions { MaxDegreeOfParallelism = 1 }, async (i, ct) =>
        {
            await Task.Delay(10, ct);
            results[i] = i * 2;
        }).GetAwaiter().GetResult();
        return results.Sum(r => (long)r);
    }
}
