namespace BatchReport;

public static class Workload
{
    public static long Run()
    {
        long checksum = 0;
        for (int batch = 0; batch < 150; batch++)
        {
            var big = new long[30_000];
            for (int i = 0; i < big.Length; i += 16) big[i] = i + batch;
            var parts = new List<byte[]>();
            for (int p = 0; p < 8; p++)
            {
                var part = new byte[40_000];
                part[batch % part.Length] = (byte)p;
                parts.Add(part);
            }
            checksum += big[16 * (batch % 100)] + parts.Count + parts[3][batch % 40_000];
            // No forced collection: the GC collects this garbage when it needs the space, not before.
        }
        return checksum;
    }
}
