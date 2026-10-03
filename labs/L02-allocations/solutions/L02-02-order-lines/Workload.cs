using System.Globalization;

namespace OrderLines;

public enum Region { Other, EU, NA, APAC }

[Flags]
public enum LineFlags { None = 0, Gift = 1, Fragile = 2, Priority = 4, Hazmat = 8 }

public readonly record struct OrderLine(int Id, Region Region, int Quantity, decimal UnitPrice, LineFlags Flags);

public static class LineParser
{
    /// <summary>Parses "id;region;qty;price;flag|flag|..." e.g. "10423; eu ;17;19.99;Gift|FRAGILE".</summary>
    public static OrderLine? Parse(string line, LineFlags disabledFlags)
    {
        var lineSpan = line.AsSpan();
        var parts = lineSpan.Split(';');

        if (!parts.MoveNext()) return null;
        if (!int.TryParse(lineSpan[parts.Current], CultureInfo.InvariantCulture, out var id))
            return null;

        if (!parts.MoveNext()) return null;
        if (!Enum.TryParse<Region>(lineSpan[parts.Current], ignoreCase: true, out var region))
            region = Region.Other;

        if (!parts.MoveNext()) return null;
        if (!int.TryParse(lineSpan[parts.Current], CultureInfo.InvariantCulture, out var quantity))
            return null;

        if (!parts.MoveNext()) return null;
        if (!decimal.TryParse(lineSpan[parts.Current], CultureInfo.InvariantCulture, out var unitPrice))
            return null;

        if (!parts.MoveNext()) return null;
        var flags = LineFlags.None;
        var flagsSpan = lineSpan[parts.Current];
        var flagParts = flagsSpan.Split('|');
        while (flagParts.MoveNext())
        {
            if (!Enum.TryParse<LineFlags>(flagsSpan[flagParts.Current], ignoreCase: true, out var flag))
                continue;

            flags |= flag;
        }
        flags &= ~disabledFlags;

        return new OrderLine(id, region, quantity, unitPrice, flags);
    }
}

public static class Workload
{
    // Generated once, on first use (the harness warms up first), so the *input* is not counted as allocation.
    static readonly string[] Lines = CreateLines(200_000);

    public static long Run()
    {
        const LineFlags disabled = LineFlags.Hazmat;   // the hazmat feature is switched off in this deployment

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
