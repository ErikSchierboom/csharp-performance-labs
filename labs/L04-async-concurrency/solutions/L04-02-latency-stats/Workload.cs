namespace LatencyStats;

public sealed class Recorder
{
    readonly long[] _buckets = new long[64];

    public void Record(int micros)
    {
        int bucket = Audit(micros) & 63;                       // pure work: no lock needed
        Interlocked.Add(ref _buckets[bucket], micros); // the only shared write, done atomically
    }

    static int Audit(int x)
    {
        uint h = (uint)x * 2654435761u;
        for (int i = 0; i < 300; i++) h = (h ^ (h >> 13)) * 0x5bd1e995u + (uint)i;
        return (int)(h >> 7);
    }

    public long Total() { long t = 0; for (int i = 0; i < _buckets.Length; i++) t += Volatile.Read(ref _buckets[i]) * (i + 1); return t; }
}

public static class Workload
{
    public static long Run()
    {
        var recorder = new Recorder();
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, worker =>
        {
            for (int i = 0; i < 40_000; i++) recorder.Record(worker * 1_000 + i % 997);
        });
        return recorder.Total();
    }
}
