using Microsoft.Extensions.Logging;

namespace DebugLogging;

public static partial class Messages
{
    // Source-generated: checks IsEnabled first and formats nothing (allocates nothing) when the level is off.
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Processing order {OrderId} for {Customer} at {Ticks}")]
    public static partial void Processing(ILogger logger, int orderId, string customer, long ticks);
}
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
            Messages.Processing(Log, i, customer, DateTime.UtcNow.Ticks);
            handled += i % 7;
        }
        return handled;
    }
}
