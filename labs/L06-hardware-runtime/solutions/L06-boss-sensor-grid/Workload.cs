namespace SensorGrid;

public struct Reading { public int Row, Col, Value; }                        // L06-02: values inline, no pointer chasing

public static class Workload
{
    const int N = 2_048;
    static readonly int[] Grid = CreateGrid();
    static readonly Reading[] Readings = CreateReadings(1_000_000);
    static readonly int[] Codes = CreateCodes(8_000_000);

    public static long Run()
    {
        long colSum = 0;
        for (int r = 0; r < N; r++)                                          // L06-01: walk in memory order
            for (int c = 0; c < N; c++)
                colSum += Grid[r * N + c];

        long weighted = 0;
        foreach (ref readonly var x in Readings.AsSpan()) weighted += x.Value * 3L + x.Row + x.Col * 2L;

        long sevens = ((ReadOnlySpan<int>)Codes).Count(7);                   // L06-08: vectorised BCL primitive

        return colSum * 31 + weighted * 7 + sevens;
    }

    static int[] CreateGrid() { var rng = new Random(1); var g = new int[N * N]; for (int i = 0; i < g.Length; i++) g[i] = rng.Next(100); return g; }
    static Reading[] CreateReadings(int n)
    {
        var a = new Reading[n];
        for (int i = 0; i < n; i++) a[i] = new Reading { Row = i % 100, Col = i % 37, Value = i % 91 };
        return a;
    }
    static int[] CreateCodes(int n) { var rng = new Random(3); var d = new int[n]; for (int i = 0; i < n; i++) d[i] = rng.Next(50); return d; }
}
