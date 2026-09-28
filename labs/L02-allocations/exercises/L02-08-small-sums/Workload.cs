namespace SmallSums;

public static class Workload
{
    static readonly List<int> Values = Enumerable.Range(0, 24).ToList();

    // A helper that "accepts anything enumerable"...
    static long Sum(IEnumerable<int> values)
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
