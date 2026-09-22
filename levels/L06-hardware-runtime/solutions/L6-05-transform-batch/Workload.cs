using System.Runtime.CompilerServices;

namespace TransformBatch;

[InlineArray(64)] public struct Cells { double _e; }

// `readonly struct`: the compiler now *knows* Score() can't modify it, so it never needs a defensive copy.
public readonly struct Transform
{
    public readonly Cells C;

    public Transform(double a, double b, double c) { C = default; Unsafe.AsRef(in C)[0] = a; Unsafe.AsRef(in C)[31] = b; Unsafe.AsRef(in C)[63] = c; }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public double Score() => C[0] * C[0] + C[63] * C[63] + C[31];
}

public static class Workload
{
    static readonly Transform Model = new(1, 0.5, 3);

    public static long Run()
    {
        double total = 0;
        for (int i = 0; i < 20_000_000; i++)
            total += Model.Score();
        return (long)total;
    }
}
