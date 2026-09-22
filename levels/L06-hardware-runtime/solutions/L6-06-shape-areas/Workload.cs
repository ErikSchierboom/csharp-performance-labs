namespace ShapeAreas;

public sealed class Rect { public readonly int W, H; public Rect(int w, int h) { W = w; H = h; } }
public sealed class Square { public readonly int S; public Square(int s) => S = s; }
public sealed class Triangle { public readonly int B, H; public Triangle(int b, int h) { B = b; H = h; } }

public static class Workload
{
    // The same data, stored grouped by concrete type. No interface, no per-element dispatch.
    static readonly Rect[] Rects;
    static readonly Square[] Squares;
    static readonly Triangle[] Triangles;

    static Workload()
    {
        var rng = new Random(6);
        var rects = new List<Rect>(); var squares = new List<Square>(); var tris = new List<Triangle>();
        for (int i = 0; i < 1_000_000; i++)
            switch (rng.Next(3))
            {
                case 0: rects.Add(new Rect(rng.Next(1, 50), rng.Next(1, 50))); break;
                case 1: squares.Add(new Square(rng.Next(1, 50))); break;
                default: tris.Add(new Triangle(rng.Next(1, 50), rng.Next(1, 50))); break;
            }
        Rects = rects.ToArray(); Squares = squares.ToArray(); Triangles = tris.ToArray();
    }

    public static long Run()
    {
        long total = 0;
        for (int pass = 0; pass < 30; pass++)
        {
            foreach (var r in Rects) total += r.W * r.H;
            foreach (var s in Squares) total += s.S * s.S;
            foreach (var t in Triangles) total += t.B * t.H / 2;
        }
        return total;
    }
}
