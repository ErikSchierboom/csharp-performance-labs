namespace InvoiceExport;

public record Order(int Id, string Customer, string Sku, int Quantity, decimal UnitPrice, DateOnly Date);

public static class InvoiceExporter
{
    public static string Export(IReadOnlyList<Order> orders)
    {
        var report = "id,customer,sku,qty,unit_price,total,date\n";

        foreach (var order in orders)
        {
            report += FormatRow(order);
        }

        report += $"# {orders.Count} rows\n";
        return report;
    }

    static string FormatRow(Order o) =>
        string.Format("{0},{1},{2},{3},{4:F2},{5:F2},{6:yyyy-MM-dd}\n",
            o.Id, o.Customer, o.Sku, o.Quantity, o.UnitPrice, o.Quantity * o.UnitPrice, o.Date);
}

public static class Workload
{
    public static long Run()
    {
        var orders = CreateOrders(5_000);
        var report = InvoiceExporter.Export(orders);

        long lines = 0;
        foreach (var ch in report) if (ch == '\n') lines++;
        return report.Length * 1_000_003L + lines;
    }

    static List<Order> CreateOrders(int n)
    {
        var rng = new Random(42);
        var skus = new[] { "WID-100", "WID-200", "GAD-310", "GAD-455", "SPR-020", "SPR-021" };
        var list = new List<Order>(n);
        for (int i = 0; i < n; i++)
        {
            list.Add(new Order(
                Id: i + 1,
                Customer: "Customer-" + rng.Next(1, 900),
                Sku: skus[rng.Next(skus.Length)],
                Quantity: rng.Next(1, 40),
                UnitPrice: rng.Next(100, 20_000) / 100m,
                Date: new DateOnly(2025, 1, 1).AddDays(rng.Next(0, 365))));
        }
        return list;
    }
}
