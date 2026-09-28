using System.Globalization;

namespace OrderLines;

public enum Region { Other, EU, NA, APAC }

[Flags]
public enum LineFlags { None = 0, Gift = 1, Fragile = 2, Priority = 4, Hazmat = 8 }

public readonly record struct OrderLine(int Id, Region Region, int Quantity, decimal UnitPrice, LineFlags Flags);

public static class LineParser
{
    /// <summary>Parses "id;region;qty;price;flag|flag|..." e.g. "10423; eu ;17;19.99;Gift|FRAGILE".</summary>
    public static OrderLine? Parse(string line, HashSet<string> disabledFlags)
    {
        var parts = line.Split(';');
        if (parts.Length != 5) return null;

        var region = parts[1].Trim().ToUpperInvariant() switch
        {
            "EU" => Region.EU,
            "NA" => Region.NA,
            "APAC" => Region.APAC,
            _ => Region.Other,
        };

        var flags = parts[4].Split('|')
            .Select(f => f.Trim().ToLowerInvariant())
            .Where(f => !disabledFlags.Contains(f))
            .Aggregate(LineFlags.None, (acc, f) => acc | ParseFlag(f));

        return new OrderLine(
            int.Parse(parts[0], CultureInfo.InvariantCulture),
            region,
            int.Parse(parts[2], CultureInfo.InvariantCulture),
            decimal.Parse(parts[3], CultureInfo.InvariantCulture),
            flags);
    }

    static LineFlags ParseFlag(string f) => f switch
    {
        "gift" => LineFlags.Gift,
        "fragile" => LineFlags.Fragile,
        "priority" => LineFlags.Priority,
        "hazmat" => LineFlags.Hazmat,
        _ => LineFlags.None,
    };
}

public static class Workload
{
    // Generated once, on first use (the harness warms up first), so the *input* is not counted as allocation.
    static readonly string[] Lines = CreateLines(200_000);

    public static long Run()
    {
        var disabled = new HashSet<string> { "hazmat" };   // the hazmat feature is switched off in this deployment

        long checksum = 0;
        int malformed = 0;
        foreach (var line in Lines)
        {
            var parsed = LineParser.Parse(line, disabled);
            if (parsed is not { } l) { malformed++; continue; }
            checksum += l.Id + (int)l.Region * 3L + l.Quantity * 5L + (long)(l.UnitPrice * 100) + (int)l.Flags * 7L;
        }
        return checksum * 1_000_003L + malformed;
    }

    static string[] CreateLines(int n)
    {
        var rng = new Random(11);
        var regions = new[] { "EU", " eu ", "NA", "apac", "APAC", "LATAM" };
        var flagNames = new[] { "Gift", "FRAGILE", "priority", "Hazmat", "gift", " fragile", "unknown" };
        var lines = new string[n];
        for (int i = 0; i < n; i++)
        {
            if (rng.Next(100) < 2) { lines[i] = "garbage line " + i; continue; }
            int flagCount = rng.Next(0, 4);
            var flags = new string[Math.Max(flagCount, 1)];
            for (int f = 0; f < flags.Length; f++) flags[f] = flagCount == 0 ? "" : flagNames[rng.Next(flagNames.Length)];
            lines[i] = string.Create(CultureInfo.InvariantCulture,
                $"{i + 1};{regions[rng.Next(regions.Length)]};{rng.Next(1, 100)};{rng.Next(100, 99_999) / 100m};{string.Join('|', flags)}");
        }
        return lines;
    }
}
