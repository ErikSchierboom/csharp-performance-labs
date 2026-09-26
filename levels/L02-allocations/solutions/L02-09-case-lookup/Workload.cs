namespace CaseLookup;

public static class Workload
{
    static readonly string[] Names = Enumerable.Range(0, 500).Select(i => "Customer-" + i).ToArray();
    // Case-insensitive comparer: the dictionary compares (and hashes) ignoring case without creating new strings.
    static readonly Dictionary<string, int> Index = Names.ToDictionary(n => n, n => n.Length * 7, StringComparer.OrdinalIgnoreCase);
    static readonly string[] Queries = CreateQueries(400_000);

    public static long Run()
    {
        long total = 0;
        foreach (var q in Queries)
            if (Index.TryGetValue(q, out int v)) total += v;
        return total;
    }

    static string[] CreateQueries(int n)
    {
        var rng = new Random(5);
        var a = new string[n];
        for (int i = 0; i < n; i++)
        {
            var name = Names[rng.Next(Names.Length)];
            a[i] = rng.Next(3) switch { 0 => name.ToUpperInvariant(), 1 => name.ToLowerInvariant(), _ => name };
        }
        return a;
    }
}
