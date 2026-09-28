using System.Buffers;

namespace PageRenderer;

public static class Renderer
{
    const int PageBytes = 100_000;      // one rasterised page: ~100 KB

    /// <summary>Rasterises a "page" (sparse glyph marks on a blank canvas) and returns a fingerprint of the canvas.</summary>
    public static long Render(int pageId)
    {
        // Rent instead of allocate: the pool hands back an array that may hold the *previous* page's marks
        // and is usually longer than asked for. So: clear what we use, and only ever look at PageBytes of it.
        var pooled = ArrayPool<byte>.Shared.Rent(PageBytes);
        try
        {
            var canvas = pooled.AsSpan(0, PageBytes);
            canvas.Clear();                                // a blank canvas: all zeros
            return Rasterise(pageId, canvas);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pooled);
        }
    }

    static long Rasterise(int pageId, Span<byte> canvas)
    {

        // Draw ~600 glyph marks at scattered positions.
        uint x = (uint)pageId * 2654435761u + 1;
        for (int i = 0; i < 600; i++)
        {
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            canvas[(int)(x % PageBytes)] = (byte)(x >> 24 | 1);
        }

        // Fingerprint: sample every 61st byte of the canvas.
        long h = 17;
        for (int i = 0; i < canvas.Length; i += 61)
            h = h * 31 + canvas[i];
        return h;
    }
}

public static class Workload
{
    public static long Run()
    {
        long checksum = 0;
        for (int page = 0; page < 6_000; page++)
            checksum += Renderer.Render(page);
        return checksum;
    }
}
