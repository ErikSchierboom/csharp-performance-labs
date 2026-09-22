namespace TileCache;

/// <summary>A small rendered map tile: 64 bytes of pixel data plus a coordinate.</summary>
public sealed class Tile
{
    static int _live;
    public static int Live => _live;          // diagnostics: how many tiles exist right now

    readonly byte[] _pixels = new byte[64];
    public int X { get; }
    public int Y { get; }

    public Tile(int x, int y)
    {
        X = x; Y = y;
        Interlocked.Increment(ref _live);
    }

    public void Paint(int seed)
    {
        for (int i = 0; i < _pixels.Length; i++)
            _pixels[i] = (byte)(seed + i * 31 + X * 7 + Y * 13);
    }

    public int Fingerprint()
    {
        int h = 17;
        for (int i = 0; i < _pixels.Length; i += 3) h = h * 31 + _pixels[i];
        return h;
    }

    // Boilerplate from the team's "disposable pattern" snippet.
    ~Tile()
    {
        Interlocked.Decrement(ref _live);
    }
}

public static class Workload
{
    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 1_500_000; i++)
        {
            var tile = new Tile(i % 512, i / 512 % 512);
            tile.Paint(i);
            checksum += tile.Fingerprint();
        }
        return checksum;
    }
}
