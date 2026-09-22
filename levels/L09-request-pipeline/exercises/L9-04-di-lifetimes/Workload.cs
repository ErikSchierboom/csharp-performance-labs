using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness.Web;

namespace DiLifetimes;

public sealed class PriceCalculator
{
    readonly Dictionary<int, int> _table;
    public PriceCalculator() => _table = Enumerable.Range(0, 20_000).ToDictionary(i => i, i => i * 3 + 1);   // "loads reference data"
    public int Price(int id) => _table[id % 20_000];
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/price/{id:int}", (int id) =>
    {
        // "resolve the calculator from a container"
        using var provider = new ServiceCollection().AddTransient<PriceCalculator>().BuildServiceProvider();
        return provider.GetRequiredService<PriceCalculator>().Price(id).ToString();
    }));

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 32, total: 1500, i => "/price/" + i);
}
