using System.Globalization;
using System.Text;

namespace ShipmentManifest;

public record Carrier(string Code, string Name);

public static class Workload
{
    static readonly List<Carrier> CarrierList = Enumerable.Range(0, 300).Select(i => new Carrier("C" + i.ToString("D3"), "Carrier " + i)).ToList();
    static readonly Dictionary<string, Carrier> Carriers = CarrierList.ToDictionary(c => c.Code);      // lookup by code: O(1)
    static readonly string[] Lines = CreateLines(20_000);

    public static long Run()
    {
        var weights = new List<double>(Lines.Length);                // no boxing
        var known = 0; var unknown = 0;
        var manifest = new StringBuilder();                          // no quadratic +=

        Span<char> upper = stackalloc char[16];
        for (int n = 0; n < Lines.Length; n++)
        {
            ReadOnlySpan<char> line = Lines[n];
            int a = line.IndexOf(';');
            var rest = line[(a + 1)..];
            int b = rest.IndexOf(';');
            var codeSpan = rest[..b].Trim();
            var rest2 = rest[(b + 1)..];
            int c = rest2.IndexOf(';');
            var weightSpan = rest2[..c];

            int len = codeSpan.ToUpperInvariant(upper);
            if (!Carriers.TryGetValue(new string(upper[..len]), out var carrier)) { unknown++; continue; }
            known++;
            weights.Add(double.Parse(weightSpan, CultureInfo.InvariantCulture));
            if (n % 8 == 0) manifest.Append(line[..a]).Append('|').Append(carrier.Name).Append('|').Append(weightSpan).Append('\n');
        }

        double total = 0;
        foreach (double w in weights) total += w;
        return known * 1_000_003L + unknown * 101L + (long)(total * 10) + manifest.Length;
    }

    static string[] CreateLines(int n)
    {
        var rng = new Random(21);
        var lines = new string[n];
        for (int i = 0; i < n; i++)
            lines[i] = $"TRK{i:D6};{(rng.Next(20) == 0 ? "zzz" : " c" + rng.Next(300).ToString("D3") + " ")};{rng.Next(1, 500) / 10.0:F1};note";
        return lines;
    }
}
