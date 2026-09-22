namespace PriceLookup;

public sealed class PriceService
{
    readonly Dictionary<int, decimal> _cache = new();

    public async Task<decimal> GetPriceAsync(int sku)
    {
        if (_cache.TryGetValue(sku, out var cached))
            return cached;                               // ~99.99% of calls end here, without ever awaiting

        var price = await LoadFromBackendAsync(sku);
        _cache[sku] = price;
        return price;
    }

    // Stand-in for a database / HTTP call that genuinely completes asynchronously.
    static async Task<decimal> LoadFromBackendAsync(int sku)
    {
        await Task.Yield();
        return 5m + sku % 97 * 0.25m;
    }
}

public static class Workload
{
    public static long Run() => RunAsync().GetAwaiter().GetResult();

    static async Task<long> RunAsync()
    {
        var service = new PriceService();
        long cents = 0;
        for (int i = 0; i < 2_000_000; i++)
        {
            decimal price = await service.GetPriceAsync(i % 300);   // only 300 distinct SKUs
            cents += (long)(price * 100);
        }
        return cents;
    }
}
