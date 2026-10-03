namespace SmallSums;

public static class Workload
{
    static readonly int[] Values = Enumerable.Range(0, 24).ToArray();

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
        for (int i = 0; i < 2_000_000; i++) total += Sum(Values);
        return total;
    }
}
