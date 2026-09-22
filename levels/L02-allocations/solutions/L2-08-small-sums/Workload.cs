namespace SmallSums;

public static class Workload
{
    static readonly List<int> Values = Enumerable.Range(0, 24).ToList();

    // Accept a span: no interface, no enumerator object, works for arrays and lists alike.
    static long Sum(ReadOnlySpan<int> values)
    {
        long s = 0;
        foreach (var v in values) s += v;
        return s;
    }

    public static long Run()
    {
        long total = 0;
        var span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(Values);
        for (int i = 0; i < 2_000_000; i++) total += Sum(span);
        return total;
    }
}
