namespace BatchReport;

public static class Workload
{
    public static long Run()
    {
        long checksum = 0;
        for (int batch = 0; batch < 150; batch++)
        {
            // Each batch builds temporary working data: one LOH-sized array and several smaller ones.
            var big = new long[30_000];                               // 240 KB: Large Object Heap
            for (int i = 0; i < big.Length; i += 16) big[i] = i + batch;
            var parts = new List<byte[]>();
            for (int p = 0; p < 8; p++)
            {
                var part = new byte[40_000];                          // 40 KB: small object heap
                part[batch % part.Length] = (byte)p;
                parts.Add(part);
            }
            checksum += big[16 * (batch % 100)] + parts.Count + parts[3][batch % 40_000];

            // "Memory kept growing in the dashboard, so we force a collection after every batch."
            GC.Collect();
        }
        return checksum;
    }
}
