namespace PriceTicker;

/// <summary>Shows a price and refreshes it in the background once a second.</summary>
public sealed class Ticker
{
    readonly byte[] _cache = new byte[20_000];
    readonly Timer _timer;
    int _ticks;
    public int Seed { get; }

    public Ticker(int seed)
    {
        Seed = seed;
        for (int i = 0; i < _cache.Length; i += 64) _cache[i] = (byte)(seed + i);
        _timer = new Timer(_ => Refresh(), null, dueTime: 1_000, period: 1_000);
    }

    void Refresh() => Interlocked.Increment(ref _ticks);

    public int Value => _cache[Seed * 64 % _cache.Length] + Seed % 13;
}

public static class Workload
{
    public static void Reset() { }

    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 1_000; i++)
        {
            var ticker = new Ticker(i);       // used briefly, then dropped
            checksum += ticker.Value;
        }
        return checksum;
    }
}
