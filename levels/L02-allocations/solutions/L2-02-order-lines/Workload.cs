using System.Globalization;

namespace OrderLines;

public enum Region { Other, EU, NA, APAC }

[Flags]
public enum LineFlags { None = 0, Gift = 1, Fragile = 2, Priority = 4, Hazmat = 8 }

public readonly record struct OrderLine(int Id, Region Region, int Quantity, decimal UnitPrice, LineFlags Flags);

public static class LineParser
{
    /// <summary>Parses "id;region;qty;price;flag|flag|..." without allocating: slice the line, don't copy it.</summary>
    public static OrderLine? Parse(ReadOnlySpan<char> line, LineFlags disabled)
    {
        // Five ';'-separated fields.
        if (!Next(ref line, ';', out var idText)) return null;
        if (!Next(ref line, ';', out var regionText)) return null;
        if (!Next(ref line, ';', out var qtyText)) return null;
        if (!Next(ref line, ';', out var priceText)) return null;
        var flagsText = line;                                  // the rest of the line
        if (flagsText.Contains(';')) return null;              // a 6th field: malformed, like Split(';').Length != 5

        var flags = LineFlags.None;
        while (true)
        {
            bool more = Next(ref flagsText, '|', out var flagText);
            if (!more) flagText = flagsText;                   // the last flag has no trailing '|'
            flags |= ParseFlag(flagText.Trim());
            if (!more) break;
        }
        flags &= ~disabled;

        return new OrderLine(
            int.Parse(idText, CultureInfo.InvariantCulture),
            ParseRegion(regionText.Trim()),
            int.Parse(qtyText, CultureInfo.InvariantCulture),
            decimal.Parse(priceText, CultureInfo.InvariantCulture),
            flags);
    }

    // Splits off the text before the next separator. Returns false (and leaves `rest` alone) if there is none.
    static bool Next(ref ReadOnlySpan<char> rest, char separator, out ReadOnlySpan<char> field)
    {
        int i = rest.IndexOf(separator);
        if (i < 0) { field = default; return false; }
        field = rest[..i];
        rest = rest[(i + 1)..];
        return true;
    }

    static Region ParseRegion(ReadOnlySpan<char> s) =>
        s.Equals("EU", StringComparison.OrdinalIgnoreCase) ? Region.EU :
        s.Equals("NA", StringComparison.OrdinalIgnoreCase) ? Region.NA :
        s.Equals("APAC", StringComparison.OrdinalIgnoreCase) ? Region.APAC : Region.Other;

    static LineFlags ParseFlag(ReadOnlySpan<char> s) =>
        s.Equals("gift", StringComparison.OrdinalIgnoreCase) ? LineFlags.Gift :
        s.Equals("fragile", StringComparison.OrdinalIgnoreCase) ? LineFlags.Fragile :
        s.Equals("priority", StringComparison.OrdinalIgnoreCase) ? LineFlags.Priority :
        s.Equals("hazmat", StringComparison.OrdinalIgnoreCase) ? LineFlags.Hazmat : LineFlags.None;
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
