namespace InvoiceExport;

public record Order(int Id, string Customer, string Sku, int Quantity, decimal UnitPrice, DateOnly Date);

public static class InvoiceExporter
{
    public static string Export(IReadOnlyList<Order> orders)
    {
        // ~64 chars/row: pre-size so the builder never has to grow.
        var sb = new System.Text.StringBuilder(orders.Count * 64 + 64);
        sb.Append("id,customer,sku,qty,unit_price,total,date\n");

        foreach (var o in orders)
        {
            sb.Append(o.Id).Append(',')
              .Append(o.Customer).Append(',')
              .Append(o.Sku).Append(',')
              .Append(o.Quantity).Append(',')
              .Append(o.UnitPrice.ToString("F2")).Append(',')
              .Append((o.Quantity * o.UnitPrice).ToString("F2")).Append(',')
              .Append(o.Date.ToString("yyyy-MM-dd")).Append('\n');
        }

        sb.Append("# ").Append(orders.Count).Append(" rows\n");
        return sb.ToString();
    }
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

    private static List<Order> CreateOrders(int n)
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
