namespace LogClassifier2;

public record LogEntry(string Level, string Service, int Status);

public static class LineParser
{
    // Same grammar as the old regex, hand-parsed over a span:
    //   <ts:\S+> <LEVEL:[A-Z]+> <svc:[\w.-]+> - <msg> [ status=ddd]
    public static LogEntry? Parse(ReadOnlySpan<char> line)
    {
        int sp1 = line.IndexOf(' ');
        if (sp1 <= 0) return null;                                   // ts: at least one non-space char
        var rest = line[(sp1 + 1)..];

        int sp2 = rest.IndexOf(' ');
        if (sp2 <= 0) return null;
        var level = rest[..sp2];
        foreach (char c in level) if (!char.IsAsciiLetterUpper(c)) return null;
        rest = rest[(sp2 + 1)..];

        int sep = rest.IndexOf(" - ", StringComparison.Ordinal);
        if (sep <= 0) return null;
        var svc = rest[..sep];
        foreach (char c in svc) if (!(char.IsAsciiLetterOrDigit(c) || c is '_' or '.' or '-')) return null;
        rest = rest[(sep + 3)..];                                    // what is left is "<msg>[ status=ddd]"

        int status = 0;
        if (rest.Length >= 11 && rest[^11..^3] is " status="
            && char.IsAsciiDigit(rest[^3]) && char.IsAsciiDigit(rest[^2]) && char.IsAsciiDigit(rest[^1]))
            status = int.Parse(rest[^3..]);

        // Two small strings per parsed line is the price of returning a class; see "Go further".
        return new LogEntry(level.ToString(), svc.ToString(), status);
    }
}

public static class Workload
{
    // Generated once, on first use (the harness warms up first), so the *input* is not counted as allocation.
    static readonly string[] Lines = CreateLines(100_000);

    public static long Run()
    {
        int errors = 0, malformed = 0;
        long statusSum = 0;
        foreach (var line in Lines)
        {
            var entry = LineParser.Parse(line);
            if (entry is null) { malformed++; continue; }
            if (entry.Level == "ERROR") errors++;
            if (entry.Status >= 500) statusSum += entry.Status;
        }
        return errors * 1_000_000_007L + malformed * 1_000_003L + statusSum;
    }

    static string[] CreateLines(int n)
    {
        var rng = new Random(99);
        var levels = new[] { "INFO", "INFO", "INFO", "WARN", "ERROR" };
        var services = new[] { "auth.api", "orders.api", "billing-worker", "gateway" };
        var statuses = new[] { 200, 201, 404, 500, 502, 503 };
        var list = new string[n];
        for (int i = 0; i < n; i++)
        {
            if (rng.Next(100) < 3) { list[i] = "### corrupted line " + i; continue; }
            var ts = $"2025-03-{rng.Next(1, 28):D2}T{rng.Next(24):D2}:{rng.Next(60):D2}:{rng.Next(60):D2}Z";
            var tail = rng.Next(2) == 0 ? $" status={statuses[rng.Next(statuses.Length)]}" : "";
            list[i] = $"{ts} {levels[rng.Next(levels.Length)]} {services[rng.Next(services.Length)]} - request {i} handled{tail}";
        }
        return list;
    }
}
