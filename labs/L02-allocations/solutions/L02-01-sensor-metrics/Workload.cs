namespace SensorMetrics;

public sealed record Summary(long Sum, int Max, int Median, int BucketCount, long BucketHash);

public static class MetricsAggregator
{
    private const int BucketWidth = 10;

    public static Summary Summarise(int[] source)
    {
        var histogram = new Dictionary<int, int>();

        var sum = 0;
        var max = 0;

        foreach (int v in source)
        {
            sum += v;
            max = Math.Max(max, v);

            var bucket = v / BucketWidth;
            if (!histogram.TryAdd(bucket, 1))
                histogram[bucket]++;
        }

        var sorted = source.Order().ToArray();
        var median = sorted[sorted.Length / 2];

        long bucketHash = 0;
        foreach (var (key, value) in histogram)
            bucketHash += key * 1_000_003L + value * 7L;

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
