using Microsoft.Extensions.Logging;

namespace LogNoise;

public static class Dummy { }
public static class Workload
{
    // Production setting: only warnings and above are recorded, so Debug/Info calls are "free"... are they?
    static readonly ILoggerFactory Factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Warning));
    static readonly ILogger Log = Factory.CreateLogger("orders");
    static readonly string[] Customers = Enumerable.Range(0, 100).Select(i => "customer-" + i).ToArray();

    public static long Run()
    {
        long handled = 0;
        for (int i = 0; i < 300_000; i++)
        {
            string customer = Customers[i % 100];
            Log.LogDebug($"Processing order {i} for {customer} at {DateTime.UtcNow.Ticks}");
            handled += i % 7;
        }
        return handled;
    }
}
