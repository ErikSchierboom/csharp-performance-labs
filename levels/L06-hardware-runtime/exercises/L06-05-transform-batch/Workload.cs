using System.Runtime.CompilerServices;

namespace TransformBatch;

[InlineArray(64)] public struct Cells {
    private double _e; } // 64 doubles = 512 bytes, stored inline

public struct Transform
{
    public Cells C;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public double Score() => C[0] * C[0] + C[63] * C[63] + C[31];
}

public static class Workload
{
    private static readonly Transform Model = Create();

    public static long Run()
    {
        double total = 0;
        for (var i = 0; i < 20_000_000; i++)
            total += Model.Score();
        return (long)total;
    }

    private static Transform Create() { var t = new Transform(); t.C[0] = 1; t.C[31] = 0.5; t.C[63] = 3; return t; }
}
