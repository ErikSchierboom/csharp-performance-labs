namespace ThresholdCount;

public static class Workload
{
    private static readonly byte[] Data = Create(1_000_000);

    public static long Run()
    {
        // Sort once. The order of the readings doesn't matter to this analysis, and now the branch is
        // "not taken" for the first half and "taken" for the second half: trivially predictable.
        var sorted = (byte[])Data.Clone();
        Array.Sort(sorted);

        long sum = 0, hits = 0;
        for (var pass = 0; pass < 60; pass++)
            foreach (var b in sorted)
                if (b >= 128) { sum += b; hits++; }
        return sum * 1_000_003L + hits;
    }

    private static byte[] Create(int n)
    {
        var rng = new Random(3);
        var d = new byte[n];
        rng.NextBytes(d);
        return d;
    }
}
