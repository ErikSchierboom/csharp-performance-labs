namespace RouteStats;

public readonly record struct RequestEvent(int Tenant, int Route, int Status);

public static class StatsCollector
{
    static readonly string[] RouteNames = Enumerable.Range(0, 40).Select(i => "/api/v1/resource" + i).ToArray();

    /// <summary>Counts requests per (tenant, route, status) and returns a fingerprint of the resulting table.</summary>
    public static long Collect(RequestEvent[] events)
    {
        var counts = new Dictionary<string, int>();

        foreach (var e in events)
        {
            string key = $"{e.Tenant}:{RouteNames[e.Route]}:{e.Status}";
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        // Read the table back: unpack each key to get at its parts.
        long fingerprint = 0;
        foreach (var (key, count) in counts)
        {
            var parts = key.Split(':');
            int tenant = int.Parse(parts[0]);
            int status = int.Parse(parts[2]);
            fingerprint += (tenant * 1_000_003L + parts[1].Length * 101L + status) * count;
        }
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
