namespace ShapeAreas;

public interface IShape { int Area(); }
public sealed class Rect(int w, int h) : IShape
{
    public int Area() => w * h; }
public sealed class Square(int s) : IShape
{
    public int Area() => s * s; }
public sealed class Triangle(int b, int h) : IShape
{
    public int Area() => b * h / 2; }

public static class Workload
{
    private static readonly IShape[] Shapes = Create(1_000_000); // a mixed bag, in random order; built once

    public static long Run()
    {
        long total = 0;
        for (var pass = 0; pass < 30; pass++)
            foreach (var s in Shapes)
                total += s.Area();
        return total;
    }

    private static IShape[] Create(int n)
    {
        var rng = new Random(6);
        var a = new IShape[n];
        for (var i = 0; i < n; i++)
            a[i] = rng.Next(3) switch { 0 => new Rect(rng.Next(1, 50), rng.Next(1, 50)), 1 => new Square(rng.Next(1, 50)), _ => new Triangle(rng.Next(1, 50), rng.Next(1, 50)) };
        return a;
    }
}
