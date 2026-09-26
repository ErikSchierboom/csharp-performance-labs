namespace Whodunit;

public readonly struct Key(int a, int b) : IEquatable<Key>
{
    public readonly int A = a, B = b;
    public bool Equals(Key o) => A == o.A && B == o.B;
    public override bool Equals(object? o) => o is Key k && Equals(k);
    public override int GetHashCode() => HashCode.Combine(A, B); // mixes both parts: well distributed
}

public static class Workload
{
    static readonly Dictionary<Key, int> Index = Build();

    public static long Run()
    {
        long total = 0;
        for (var pass = 0; pass < 40; pass++)
            for (var i = 0; i < 50_000; i++)
            {
                var key = new Key(i % 250, i % 200);
                if (IsValid(key) && Index.TryGetValue(key, out var v)) total += v;
            }
        return total;
    }

    private static bool IsValid(Key k) => k is { A: >= 0, B: >= 0 };

    private static Dictionary<Key, int> Build()
    {
        var d = new Dictionary<Key, int>();
        for (var a = 0; a < 250; a++) for (var b = 0; b < 200; b++) d[new Key(a, b)] = a * 1000 + b;
        return d;
    }
}
