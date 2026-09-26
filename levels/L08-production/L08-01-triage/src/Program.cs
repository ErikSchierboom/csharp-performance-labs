// Mystery service for Lab L08-01. Don't read this until you've classified all five scenarios from the outside.
using System.Diagnostics;

string scenario = args.Length > 1 && args[0] == "--scenario" ? args[1] : "healthy";
int seconds = args.Contains("--seconds") ? int.Parse(args[Array.IndexOf(args, "--seconds") + 1]) : 600;
Console.WriteLine($"service pid={Environment.ProcessId} scenario=<hidden> running for up to {seconds}s (Ctrl-C to stop)");
var until = Stopwatch.GetTimestamp() + seconds * Stopwatch.Frequency;
bool Running() => Stopwatch.GetTimestamp() < until;

switch (scenario)
{
    case "a": Busy(); break;
    case "b": Churn(); break;
    case "c": Starve(); break;
    case "d": Contend(); break;
    default: Idle(); break;
}

void Busy()   // a
{
    var threads = Enumerable.Range(0, 4).Select(_ => new Thread(() => { double x = 1; while (Running()) for (int i = 0; i < 100_000; i++) x = Math.Sqrt(x + i); GC.KeepAlive(x); })).ToList();
    threads.ForEach(t => t.Start()); threads.ForEach(t => t.Join());
}

void Churn()  // b
{
    var keep = new byte[50][];
    var rng = new Random(1);
    while (Running())
    {
        for (int i = 0; i < 20; i++) keep[rng.Next(keep.Length)] = new byte[rng.Next(100_000, 300_000)];   // big, mostly short-lived
        Thread.Sleep(1);
    }
}

void Starve() // c
{
    ThreadPool.SetMinThreads(4, 4);
    async Task<int> Db(int i) { await Task.Delay(20); return i; }
    while (Running())
    {
        var burst = Enumerable.Range(0, 200).Select(i => Task.Run(() => Db(i).Result)).ToArray();
        Task.WaitAll(burst);
        Thread.Sleep(50);
    }
}

void Contend() // d
{
    var gate = new object();
    var threads = Enumerable.Range(0, 16).Select(_ => new Thread(() => { while (Running()) lock (gate) { Thread.Sleep(2); } })).ToList();
    threads.ForEach(t => t.Start()); threads.ForEach(t => t.Join());
}

void Idle()   // e (default)
{
    async Task Loop() { while (Running()) await Task.Delay(100); }
    Task.WaitAll(Enumerable.Range(0, 200).Select(_ => Loop()).ToArray());
}
