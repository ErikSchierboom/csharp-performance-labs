namespace SensorGrid;

public sealed class Reading { public int Row, Col, Value; }

public static class Workload
{
    const int N = 2_048;
    static readonly int[] Grid = CreateGrid();                               // N x N, row by row
    static readonly Reading[] Readings = CreateReadings(1_000_000);          // sparse readings, scattered on the heap
    static readonly int[] Codes = CreateCodes(8_000_000);

    public static long Run()
    {
        // 1. Column totals of the grid.
        long colSum = 0;
        for (int c = 0; c < N; c++)
            for (int r = 0; r < N; r++)
                colSum += Grid[r * N + c];

        // 2. Weighted sum over the sparse readings.
        long weighted = 0;
        foreach (var x in Readings) weighted += x.Value * 3L + x.Row + x.Col * 2L;

        // 3. How many codes equal 7?
        long sevens = Codes.Count(k => k == 7);

        return colSum * 31 + weighted * 7 + sevens;
    }

    static int[] CreateGrid() { var rng = new Random(1); var g = new int[N * N]; for (int i = 0; i < g.Length; i++) g[i] = rng.Next(100); return g; }
    static Reading[] CreateReadings(int n)
    {
        var rng = new Random(2);
        var a = new Reading[n];
        for (int i = 0; i < n; i++) a[i] = new Reading { Row = i % 100, Col = i % 37, Value = i % 91 };
        for (int i = n - 1; i > 0; i--) { int j = rng.Next(i + 1); (a[i], a[j]) = (a[j], a[i]); }
        return a;
    }
    static int[] CreateCodes(int n) { var rng = new Random(3); var d = new int[n]; for (int i = 0; i < n; i++) d[i] = rng.Next(50); return d; }
}
