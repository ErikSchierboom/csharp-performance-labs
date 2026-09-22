using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderLedger;

public record Customer(int Id, string Name);
public record Row(int Order, int Customer, decimal Amount);

public static class Workload
{
    static readonly List<Customer> Customers = Enumerable.Range(1, 300).Select(i => new Customer(i, "Customer " + i)).ToList();
    static readonly string[] Lines = CreateLines(5_000);                // input, built once

    static readonly Regex OrderId = new(@"^ORD-(\d+)$", RegexOptions.Compiled);          // built once

    static Row? ParseLine(string line)
    {
        var parts = line.Split(';');
        var m = OrderId.Match(parts[0]);
        if (!m.Success) return null;
        if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)) return null;      // no exception
        return new Row(int.Parse(m.Groups[1].Value), int.Parse(parts[1]), amount);
    }

    public static long Run()
    {
        var rows = Lines.Select(ParseLine).Where(r => r is not null).Select(r => r!).ToList();     // evaluate once
        if (rows.Count == 0) return 0;
        long count = rows.Count;
        decimal total = rows.Sum(r => r.Amount);

        var byId = Customers.ToDictionary(c => c.Id);                                              // O(1) lookup
        var report = new StringBuilder();
        int unknown = 0;
        foreach (var r in rows)
        {
            if (!byId.TryGetValue(r.Customer, out var c)) { unknown++; continue; }
            report.Append(r.Order).Append('|').Append(c.Name).Append('|').Append(r.Amount).Append('\n');
        }
        return count * 1_000_003L + (long)(total * 100) + report.Length + unknown * 7L;
    }

    static string[] CreateLines(int n)
    {
        var rng = new Random(41);
        var a = new string[n];
        for (int i = 0; i < n; i++)
            a[i] = $"ORD-{i:D5};{(rng.Next(25) == 0 ? 999 : rng.Next(1, 301))};{(rng.Next(12) == 0 ? "n/a" : (rng.Next(100, 99_999) / 100m).ToString(CultureInfo.InvariantCulture))};paid";
        return a;
    }
}
