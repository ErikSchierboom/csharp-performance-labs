namespace ReferenceData;

/// <summary>Read-mostly reference data. Copy-on-write: readers never lock, writers replace the whole snapshot.</summary>
public sealed class ReferenceCache
{
    readonly object _writeGate = new();
    volatile Dictionary<int, int> _snapshot = new();

    public ReferenceCache()
    {
        var m = new Dictionary<int, int>();
        for (int i = 0; i < 1_000; i++) m[i] = i * 3;
        _snapshot = m;
    }

    // The snapshot is never modified after it is published, so reading it needs no lock.
    public int Get(int key) => _snapshot[key];

    public void Set(int key, int value)
    {
        lock (_writeGate)
        {
            var copy = new Dictionary<int, int>(_snapshot) { [key] = value };
            _snapshot = copy;                                       // publish atomically
        }
    }
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
            for (int j = 0; j < 250_000; j++)
            {
                local += cache.Get(j % 500);
                if (j % 25_000 == 0) cache.Set(500 + j % 400, j);
            }
            Interlocked.Add(ref total, local);
        });
        return total;
    }
}
