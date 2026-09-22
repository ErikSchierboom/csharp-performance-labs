namespace ParticleSweep;

public struct Particle
{
    public int X, Y, Vx, Vy, Id;
}

public static class Workload
{
    static readonly Particle[] Particles = Create(2_000_000);

    public static long Run()
    {
        long sum = 0;
        foreach (ref readonly var p in Particles.AsSpan())
            sum += p.X + p.Vx * 3 + p.Y * 5 + p.Vy * 7 + p.Id;
        return sum;
    }

    static Particle[] Create(int n)
    {
        var all = new Particle[n];
        for (int i = 0; i < n; i++) all[i] = new Particle { X = i, Y = i * 2, Vx = i % 7, Vy = i % 11, Id = i };
        return all;                          // the values live inline in the array: no scattering is possible
    }
}
