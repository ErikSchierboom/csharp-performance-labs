namespace MatrixWalk;

public static class Workload
{
    const int N = 4_096;
    static readonly int[] Grid = Create();

    public static long Run()
    {
        long sum = 0;
        for (int row = 0; row < N; row++)              // walk in the order the data is laid out in memory
            for (int col = 0; col < N; col++)
                sum += Grid[row * N + col];
        return sum;
    }

    static int[] Create()
    {
        var rng = new Random(1);
        var g = new int[N * N];
        for (int i = 0; i < g.Length; i++) g[i] = rng.Next(1000);
        return g;
    }
}
