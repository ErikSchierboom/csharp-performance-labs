using Microsoft.Extensions.Logging;

namespace DebugLogging;

public static class Workload
{
    private static readonly ILoggerFactory Factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Warning));
    private static readonly ILogger Log = Factory.CreateLogger("orders");
    private static readonly string[] Customers = Enumerable.Range(0, 100).Select(i => "customer-" + i).ToArray();

    public static long Run()
    {
        long handled = 0;
        for (var i = 0; i < 300_000; i++)
        {
            var customer = Customers[i % 100];
            Log.LogDebug($"Processing order {i} for {customer} at {DateTime.UtcNow.Ticks}");
            handled += i % 7;
        }
        return handled;
    }
}
