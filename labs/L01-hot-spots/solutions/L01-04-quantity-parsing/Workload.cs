using System.Globalization;

namespace QuantityParsing;

public static class QuantityParser
{
    /// <summary>Parses every valid number in the input; anything unparseable is skipped.</summary>
    public static List<decimal> ParseAll(IEnumerable<string> raw)
    {
        var result = new List<decimal>();

        foreach (var s in raw)
        {
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                result.Add(value);
        }

        return result;
    }
}

public static class Workload
{
    public static long Run()
    {
        var cells = CreateCells(1_000_000);
        var values = QuantityParser.ParseAll(cells);

        decimal total = 0;
        foreach (var v in values) total += v;
        return values.Count * 1_000_003L + (long)(total * 100);
    }

    static List<string> CreateCells(int n)
    {
        var rng = new Random(2024);
        var junk = new[] { "N/A", "", "--", "n/a", "TBD", "see note" };
        var list = new List<string>(n);
        for (int i = 0; i < n; i++)
        {
            list.Add(rng.Next(2) == 0
                ? (rng.Next(1, 1_000_000) / 100m).ToString("F2", CultureInfo.InvariantCulture)
                : junk[rng.Next(junk.Length)]);
        }
        return list;
    }
}
