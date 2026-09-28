namespace CountMatches;

public static class Workload
{
    private static readonly int[] Data = Create(256_000);
    private const int Passes = 400;

    public static long Run()
    {
        long total = 0;
        for (var pass = 0; pass < Passes; pass++)
            total += Data.Count(x => x == 42);
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
