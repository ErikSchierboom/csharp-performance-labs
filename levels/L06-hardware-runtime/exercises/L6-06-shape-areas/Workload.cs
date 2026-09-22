namespace ShapeAreas;

public interface IShape { int Area(); }
public sealed class Rect : IShape { readonly int _w, _h; public Rect(int w, int h) { _w = w; _h = h; } public int Area() => _w * _h; }
public sealed class Square : IShape { readonly int _s; public Square(int s) => _s = s; public int Area() => _s * _s; }
public sealed class Triangle : IShape { readonly int _b, _h; public Triangle(int b, int h) { _b = b; _h = h; } public int Area() => _b * _h / 2; }

public static class Workload
{
    static readonly IShape[] Shapes = Create(1_000_000);        // a mixed bag, in random order; built once

    public static long Run()
    {
        long total = 0;
        for (int pass = 0; pass < 30; pass++)
            foreach (var s in Shapes)
                total += s.Area();
        return total;
    }

    static IShape[] Create(int n)
    {
        var rng = new Random(6);
        var a = new IShape[n];
        for (int i = 0; i < n; i++)
            a[i] = rng.Next(3) switch { 0 => new Rect(rng.Next(1, 50), rng.Next(1, 50)), 1 => new Square(rng.Next(1, 50)), _ => new Triangle(rng.Next(1, 50), rng.Next(1, 50)) };
        return a;
    }
}
