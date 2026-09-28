namespace BitCounts;

public static class Workload
{
    private static readonly ulong[] Words = Create(4_000_000);

    public static long Run()
    {
        long bits = 0;
        foreach (var w in Words)
        {
            var x = w;
            while (x != 0) { x &= x - 1; bits++; } // clear the lowest set bit until none remain
        }
        return bits;
    }

    private static ulong[] Create(int n) { var rng = new Random(9); var a = new ulong[n]; for (int i = 0; i < n; i++) a[i] = (ulong)rng.NextInt64() ^ ((ulong)rng.NextInt64() << 1); return a; }
}
