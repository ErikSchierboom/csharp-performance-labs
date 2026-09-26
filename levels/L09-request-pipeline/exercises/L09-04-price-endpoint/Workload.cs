using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness.Web;

namespace PriceEndpoint;

/// <summary>Stands in for <c>prices.csv</c>, shipped with the app: 2,000 rows of <c>productId,unitPrice</c>.</summary>
public static class PriceFile
{
    public static readonly string Contents = Build();

    private static string Build()
    {
        var sb = new StringBuilder();
        for (var id = 0; id < 2_000; id++)
            sb.Append(id).Append(',').Append((id * 3 + 1) / 100m).Append('\n');
        return sb.ToString();
    }
}

/// <summary>The price list, parsed into a lookup table when the catalog is created.</summary>
public sealed class PriceCatalog
{
    private readonly Dictionary<int, decimal> _prices = new();

    public PriceCatalog()
    {
        foreach (var line in PriceFile.Contents.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = line.Split(',');
            _prices[int.Parse(fields[0], CultureInfo.InvariantCulture)] = decimal.Parse(fields[1], CultureInfo.InvariantCulture);
        }
    }

    public decimal UnitPrice(int productId) => _prices[productId];
}

public sealed class PricingService(PriceCatalog catalog)
{
    // 10% off from 10 items up.
    public decimal Quote(int productId, int quantity) =>
        catalog.UnitPrice(productId) * quantity * (quantity >= 10 ? 0.9m : 1m);
}

public static class Workload
{
    private static readonly WebRig Rig = WebRig.Start(
        app => app.MapGet("/price/{id:int}", (int id, int qty, PricingService pricing) =>
            pricing.Quote(id % 2_000, qty).ToString(CultureInfo.InvariantCulture)),
        builder =>
        {
            builder.Services.AddTransient<PricingService>();
            builder.Services.AddTransient<PriceCatalog>();
        });

    public static long Run() => Rig.Drive(users: 32, total: 1500, i => $"/price/{i * 7}?qty={1 + i % 12}");
}
