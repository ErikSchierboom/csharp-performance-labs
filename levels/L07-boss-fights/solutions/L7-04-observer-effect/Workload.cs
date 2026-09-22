namespace ObserverEffect;

public readonly struct Key : IEquatable<Key>
{
    public readonly int A, B;
    public Key(int a, int b) { A = a; B = b; }
    public bool Equals(Key o) => A == o.A && B == o.B;
    public override bool Equals(object? o) => o is Key k && Equals(k);
    public override int GetHashCode() => HashCode.Combine(A, B);    // mixes both parts: well distributed
}

public static class Workload
{
    static readonly Dictionary<Key, int> Index = Build();

    public static long Run()
    {
        long total = 0;
        for (int pass = 0; pass < 40; pass++)
            for (int i = 0; i < 50_000; i++)
            {
                var key = new Key(i % 250, i % 200);
                if (IsValid(key) && Index.TryGetValue(key, out int v)) total += v;
            }
        return total;
    }

    // Tiny, called constantly, inlined by the JIT in optimised code.
    static bool IsValid(Key k) => k.A >= 0 && k.B >= 0;

    static Dictionary<Key, int> Build()
    {
        var d = new Dictionary<Key, int>();
        for (int a = 0; a < 250; a++) for (int b = 0; b < 200; b++) d[new Key(a, b)] = a * 1000 + b;
        return d;
    }
}
