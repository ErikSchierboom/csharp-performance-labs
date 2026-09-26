namespace CustomerDashboard;

public record Customer(int Id, string Region, int Orders, double Spend, bool Active);
public record Scored(Customer Customer, double Score);
public record Summary(int ActiveCount, double TotalScore, int TopCustomerId);

public static class DashboardBuilder
{
    public static Summary Build(IEnumerable<Customer> customers)
    {
        var scored = customers
            .Where(c => c.Active)
            .Select(c => new Scored(c, ComputeScore(c)))
            .ToList();                       // enumerate (and score) exactly once

        if (scored.Count == 0)
            return new Summary(0, 0, -1);

        var count = scored.Count;
        var total = scored.Sum(s => s.Score);
        var top = scored.OrderByDescending(s => s.Score).First();

        return new Summary(count, total, top.Customer.Id);
    }

    // Stand-in for a "score" that is genuinely expensive to compute.
    static double ComputeScore(Customer c)
    {
        double s = c.Spend;
        for (int i = 0; i < 300; i++)
            s = Math.Sqrt(s + c.Orders * 0.5 + i);
        return s;
    }
}

public static class Workload
{
    public static long Run()
    {
        var customers = CreateCustomers(40_000);
        var summary = DashboardBuilder.Build(customers);
        return summary.ActiveCount * 1_000_003L + summary.TopCustomerId + (long)Math.Round(summary.TotalScore * 1000);
    }

    static List<Customer> CreateCustomers(int n)
    {
        var rng = new Random(5);
        var regions = new[] { "EU", "NA", "APAC", "LATAM" };
        var list = new List<Customer>(n);
        for (int i = 0; i < n; i++)
            list.Add(new Customer(i, regions[rng.Next(regions.Length)], rng.Next(1, 200), rng.NextDouble() * 10_000, rng.Next(10) < 7));
        return list;
    }
}
