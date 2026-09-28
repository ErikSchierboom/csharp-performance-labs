namespace CountMatches;

public static class Workload
{
    private static readonly int[] Data = Create(256_000); // 1 MB, so it stays in the CPU cache; built once, not measured
    private const int Passes = 400;

    public static long Run()
    {
        long total = 0;
        // The BCL's span Count is vectorised (SIMD): it compares several ints per instruction.
        for (var pass = 0; pass < Passes; pass++)
            total += ((ReadOnlySpan<int>)Data).Count(42);
        return total;
    }

    private static int[] Create(int n)
    {
        var rng = new Random(7);
        var d = new int[n];
        for (var i = 0; i < n; i++) d[i] = rng.Next(100);
        return d;
    }
}
