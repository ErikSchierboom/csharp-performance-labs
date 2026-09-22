namespace ThresholdCount;

public static class Workload
{
    static readonly byte[] Data = Create(1_000_000);      // random readings, built once

    public static long Run()
    {
        long sum = 0, hits = 0;
        for (int pass = 0; pass < 60; pass++)              // the same readings are analysed for 60 different windows
            foreach (byte b in Data)
                if (b >= 128) { sum += b; hits++; }
        return sum * 1_000_003L + hits;
    }

    static byte[] Create(int n)
    {
        var rng = new Random(3);
        var d = new byte[n];
        rng.NextBytes(d);
        return d;
    }
}
