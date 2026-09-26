using System.Runtime.CompilerServices;

namespace WarmupCurve;

public static class Workload
{
    static readonly int[] Data = Enumerable.Range(0, 200_000).Select(i => (i * 2654435761u) % 1000 is var v ? (int)v : 0).ToArray();

    public static long Run()
    {
        long a = Checksum(Data);                                       // a hot loop (one long-running method)
        long b = Data.Where(x => x % 3 == 0).Select(x => x * 2L).Sum(); // a LINQ pipeline (generic, delegate-heavy)
        var groups = new Dictionary<int, int>();
        foreach (int x in Data) groups[x % 64] = groups.GetValueOrDefault(x % 64) + 1;
        long c = 0; foreach (var kv in groups) c += kv.Key * 31L + kv.Value;
        return a * 1_000_003L + b + c;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]   // skip tiering for this method: fully optimised from the first call
    static long Checksum(int[] data)
    {
        long h = 17;
        for (int rep = 0; rep < 40; rep++)
            for (int i = 0; i < data.Length; i++) h = h * 31 + data[i];
        return h;
    }
}
