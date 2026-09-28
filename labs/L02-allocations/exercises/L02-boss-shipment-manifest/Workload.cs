using System.Collections;
using System.Globalization;

namespace ShipmentManifest;

public record Carrier(string Code, string Name);

public static class Workload
{
    static readonly List<Carrier> Carriers = Enumerable.Range(0, 300).Select(i => new Carrier("C" + i.ToString("D3"), "Carrier " + i)).ToList();
    static readonly string[] Lines = CreateLines(20_000); // input: built once

    public static long Run()
    {
        var weights = new ArrayList();
        var known = 0; var unknown = 0;
        var manifest = "";

        for (int n = 0; n < Lines.Length; n++)
        {
            var parts = Lines[n].Split(';');
            var code = parts[1].Trim().ToUpperInvariant();
            var carrier = Carriers.FirstOrDefault(c => c.Code == code);
            if (carrier is null) { unknown++; continue; }
            known++;
            weights.Add(double.Parse(parts[2], CultureInfo.InvariantCulture));
            if (n % 8 == 0) manifest += parts[0] + "|" + carrier.Name + "|" + parts[2] + "\n";
        }

        double total = 0;
        foreach (object w in weights) total += (double)w;
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
