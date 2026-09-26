using System.Runtime.CompilerServices;

namespace TransformBatch;

[InlineArray(64)] public struct Cells {
    private double _e; }

// `readonly struct`: the compiler now *knows* Score() can't modify it, so it never needs a defensive copy.
public readonly struct Transform
{
    private readonly Cells _c;

    public Transform(double a, double b, double c) { _c = default; Unsafe.AsRef(in _c)[0] = a; Unsafe.AsRef(in _c)[31] = b; Unsafe.AsRef(in _c)[63] = c; }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public double Score() => _c[0] * _c[0] + _c[63] * _c[63] + _c[31];
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
