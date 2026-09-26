namespace PageRenderer;

public static class Renderer
{
    const int PageBytes = 100_000; // one rasterised page: ~100 KB

    /// <summary>Rasterises a "page" (sparse glyph marks on a blank canvas) and returns a fingerprint of the canvas.</summary>
    public static long Render(int pageId)
    {
        var canvas = new byte[PageBytes]; // a blank canvas: all zeros

        // Draw ~600 glyph marks at scattered positions.
        uint x = (uint)pageId * 2654435761u + 1;
        for (int i = 0; i < 600; i++)
        {
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            canvas[x % PageBytes] = (byte)(x >> 24 | 1);
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
