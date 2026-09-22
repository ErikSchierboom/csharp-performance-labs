using System.Collections;

namespace MetricsBoxing;

public sealed record Summary(long Sum, int Max, int Median, int BucketCount, long BucketHash);

public static class MetricsAggregator
{
    const int BucketWidth = 10;

    public static Summary Summarise(IEnumerable<int> source)
    {
        // Legacy-style collections: this code was ported from a .NET 1.x utility library.
        var values = new ArrayList();
        var histogram = new Hashtable();

        foreach (int v in source)
        {
            values.Add(v);

            int bucket = v / BucketWidth;
            object? current = histogram[bucket];
            histogram[bucket] = current is null ? 1 : (int)current + 1;
        }

        long sum = 0;
        int max = int.MinValue;
        foreach (object o in values)
        {
            int v = (int)o;
            sum += v;
            if (v > max) max = v;
        }

        values.Sort();
        int median = (int)values[values.Count / 2]!;

        long bucketHash = 0;
        foreach (DictionaryEntry e in histogram)
            bucketHash += (int)e.Key * 1_000_003L + (int)e.Value! * 7L;

        return new Summary(sum, max, median, histogram.Count, bucketHash);
    }
}

public static class Workload
{
    // Generated once, on first use. The harness warms up before it measures, so this
    // *input* data is not part of the measured allocation. Only the algorithm is.
    static readonly int[] Readings = CreateReadings(400_000);

    public static long Run()
    {
        var s = MetricsAggregator.Summarise(Readings);
        return s.Sum * 31 + s.Max * 17L + s.Median * 13L + s.BucketCount * 11L + s.BucketHash;
    }

    static int[] CreateReadings(int n)
    {
        var rng = new Random(2024);
        var data = new int[n];
        for (int i = 0; i < n; i++)
            data[i] = (int)Math.Abs(rng.NextDouble() * rng.NextDouble() * 5_000);   // skewed towards small values
        return data;
    }
}
