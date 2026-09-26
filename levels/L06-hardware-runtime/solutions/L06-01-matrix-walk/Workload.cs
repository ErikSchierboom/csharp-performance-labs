namespace MatrixWalk;

public static class Workload
{
    private const int N = 4_096;
    private static readonly int[] Grid = Create();

    public static long Run()
    {
        long sum = 0;
        for (var row = 0; row < N; row++) // walk in the order the data is laid out in memory
            for (var col = 0; col < N; col++)
                sum += Grid[row * N + col];
        return sum;
    }

    private static int[] Create()
    {
        var rng = new Random(1);
        var g = new int[N * N];
        for (var i = 0; i < g.Length; i++) g[i] = rng.Next(1000);
        return g;
    }
}
