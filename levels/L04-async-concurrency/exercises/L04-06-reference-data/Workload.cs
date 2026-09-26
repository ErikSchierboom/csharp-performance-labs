namespace ReferenceData;

/// <summary>Read-mostly reference data: read constantly by every request, updated rarely.</summary>
public sealed class ReferenceCache
{
    private readonly Lock _gate = new();
    private readonly Dictionary<int, int> _map = new();

    public ReferenceCache() { for (var i = 0; i < 1_000; i++) _map[i] = i * 3; }

    public int Get(int key) { lock (_gate) return _map[key]; }
    public void Set(int key, int value) { lock (_gate) _map[key] = value; }
}

public static class Workload
{
    public static long Run()
    {
        var cache = new ReferenceCache();
        long total = 0;
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, worker =>
        {
            long local = 0;
            for (var j = 0; j < 250_000; j++)
            {
                local += cache.Get(j % 500);
                if (j % 25_000 == 0) cache.Set(500 + j % 400, j);
            }
            Interlocked.Add(ref total, local);
        });
        return total;
    }
}
