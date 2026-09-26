namespace EventHub;

public sealed class Hub
{
    public event Action<int>? Published;
    public void Publish(int message) => Published?.Invoke(message);
}

/// <summary>A short-lived UI-ish object that listens to the application-wide hub.</summary>
public sealed class Widget
{
    readonly byte[] _buffer = new byte[10_000];
    public int Received { get; private set; }

    public Widget(Hub hub) => hub.Published += OnMessage;

    void OnMessage(int message)
    {
        Received++;
        _buffer[message % _buffer.Length] = 1;
    }
}

public static class Workload
{
    static Hub _hub = new(); // application-wide, lives as long as the process
    public static void Reset() => _hub = new Hub(); // test scaffolding only

    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 2_000; i++)
        {
            var widget = new Widget(_hub); // shown briefly, then dropped
            _hub.Publish(i);
            checksum += widget.Received * 3 + i % 7;
        }
        return checksum;
    }
}
