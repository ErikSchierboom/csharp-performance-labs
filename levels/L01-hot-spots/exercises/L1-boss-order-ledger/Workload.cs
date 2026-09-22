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

    // Parses one line; returns null for lines it can't read.
    static Row? ParseLine(string line)
    {
        var parts = line.Split(';');
        var m = new Regex(@"^ORD-(\d+)$").Match(parts[0]);
        if (!m.Success) return null;
        decimal amount;
        try { amount = decimal.Parse(parts[2], CultureInfo.InvariantCulture); }       // "n/a" throws
        catch (FormatException) { return null; }
        return new Row(int.Parse(m.Groups[1].Value), int.Parse(parts[1]), amount);
    }

    public static long Run()
    {
        var rows = Lines.Select(ParseLine).Where(r => r is not null).Select(r => r!);      // a lazy query...
        if (!rows.Any()) return 0;
        long count = rows.Count();                                                        // ...enumerated again
        decimal total = rows.Sum(r => r.Amount);                                          // ...and again

        var report = "";
        int unknown = 0;
        foreach (var r in rows)                                                            // ...and again
        {
            var c = Customers.FirstOrDefault(x => x.Id == r.Customer);                    // linear search per row
            if (c is null) { unknown++; continue; }
            report += r.Order + "|" + c.Name + "|" + r.Amount + "\n";                      // quadratic string building
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
