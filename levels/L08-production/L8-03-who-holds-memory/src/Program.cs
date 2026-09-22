// Mystery service for Lab L8-03. Find what holds the memory from a dump / gcdump, not from this file.
using System.Diagnostics;
int seconds = args.Contains("--seconds") ? int.Parse(args[Array.IndexOf(args, "--seconds") + 1]) : 120;
Console.WriteLine($"service pid={Environment.ProcessId} running for up to {seconds}s (Ctrl-C to stop)");
var until = Stopwatch.GetTimestamp() + seconds * Stopwatch.Frequency;
var rng = new Random(5);
int i = 0;
while (Stopwatch.GetTimestamp() < until)
{
    Requests.Handle(i++, rng);
    Thread.Sleep(1);                                   // ~1,000 requests per second
}

public sealed class Catalog { public readonly byte[][] Pages = Enumerable.Range(0, 40).Select(_ => new byte[500_000]).ToArray(); }   // big but bounded, loaded once
public sealed class Session { public readonly Guid Id = Guid.NewGuid(); public readonly byte[] Payload = new byte[8_000]; public readonly List<string> Trail = new(); }
public sealed class Audit { public readonly string User, Action; public Audit(string u, string a) { User = u; Action = a; } }

public static class Requests
{
    static readonly Catalog Cat = new();                                   // (decoy) large, but it never grows
    static readonly Dictionary<Guid, Session> ActiveSessions = new();      // never removes anything
    static readonly Queue<Audit> RecentAudit = new();                      // bounded to 1,000

    public static void Handle(int i, Random rng)
    {
        var s = new Session();
        s.Trail.Add("login"); s.Trail.Add("browse-" + rng.Next(100));
        ActiveSessions[s.Id] = s;                                          // the leak
        RecentAudit.Enqueue(new Audit("user" + i % 50, "login"));
        while (RecentAudit.Count > 1_000) RecentAudit.Dequeue();
        _ = Cat.Pages[i % 40][0];
    }
}
