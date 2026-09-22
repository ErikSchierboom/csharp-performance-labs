namespace RouteStats;

public readonly record struct RequestEvent(int Tenant, int Route, int Status);

public static class StatsCollector
{
    static readonly string[] RouteNames = Enumerable.Range(0, 40).Select(i => "/api/v1/resource" + i).ToArray();

    // A record struct gets value equality and a good hash code for free, and IEquatable<T>, so Dictionary
    // compares it without boxing. (A hand-rolled struct that forgets IEquatable<T> would box on every lookup.)
    readonly record struct Key(int Tenant, int Route, int Status);

    public static long Collect(RequestEvent[] events)
    {
        var counts = new Dictionary<Key, int>();

        foreach (var e in events)
        {
            var key = new Key(e.Tenant, e.Route, e.Status);
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        long fingerprint = 0;
        foreach (var (key, count) in counts)
            fingerprint += (key.Tenant * 1_000_003L + RouteNames[key.Route].Length * 101L + key.Status) * count;
        return fingerprint * 1_000 + counts.Count;
    }
}

public static class Workload
{
    // Generated once, on first use (the harness warms up first), so the *input* is not counted as allocation.
    static readonly RequestEvent[] Events = CreateEvents(1_000_000);

    public static long Run() => StatsCollector.Collect(Events);

    static RequestEvent[] CreateEvents(int n)
    {
        var rng = new Random(77);
        var statuses = new[] { 200, 200, 200, 201, 404, 500 };
        var events = new RequestEvent[n];
        for (int i = 0; i < n; i++)
            events[i] = new RequestEvent(rng.Next(200), rng.Next(40), statuses[rng.Next(statuses.Length)]);
        return events;
    }
}
