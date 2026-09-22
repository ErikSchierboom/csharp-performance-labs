namespace ParticleSweep;

public sealed class Particle
{
    public int X, Y, Vx, Vy, Id;
}

public static class Workload
{
    static readonly Particle[] Particles = Create(2_000_000);      // built once, not measured

    public static long Run()
    {
        long sum = 0;
        foreach (var p in Particles)
            sum += p.X + p.Vx * 3 + p.Y * 5 + p.Vy * 7 + p.Id;
        return sum;
    }

    static Particle[] Create(int n)
    {
        var rng = new Random(2);
        var all = new Particle[n];
        for (int i = 0; i < n; i++) all[i] = new Particle { X = i, Y = i * 2, Vx = i % 7, Vy = i % 11, Id = i };
        // In a real program objects end up scattered as they are created, moved and dropped over time. Simulate that.
        for (int i = n - 1; i > 0; i--) { int j = rng.Next(i + 1); (all[i], all[j]) = (all[j], all[i]); }
        return all;
    }
}
