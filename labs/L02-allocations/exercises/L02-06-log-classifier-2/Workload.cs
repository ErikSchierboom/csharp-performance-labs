using System.Text.RegularExpressions;

namespace LogClassifier2;

public record LogEntry(string Level, string Service, int Status);

public static class LineParser
{
    // This is where Lab 1 (L01-03) left off: the regex is built once, and it is still the biggest allocator.
    static readonly Regex LineRegex = new(
        @"^(?<ts>\S+) (?<level>[A-Z]+) (?<svc>[\w.-]+) - (?<msg>.*?)(?: status=(?<status>\d{3}))?$",
        RegexOptions.Compiled);

    public static LogEntry? Parse(string line)
    {
        var m = LineRegex.Match(line);
        if (!m.Success) return null;

        int status = m.Groups["status"].Success ? int.Parse(m.Groups["status"].Value) : 0;
        return new LogEntry(m.Groups["level"].Value, m.Groups["svc"].Value, status);
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
