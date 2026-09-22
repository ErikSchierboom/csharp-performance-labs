namespace CountMatches;

public static class Workload
{
    static readonly int[] Data = Create(16_000_000);          // built once, not measured

    public static long Run() => Data.Count(x => x == 42);

    static int[] Create(int n)
    {
        var rng = new Random(7);
        var d = new int[n];
        for (int i = 0; i < n; i++) d[i] = rng.Next(100);
        return d;
    }
}
