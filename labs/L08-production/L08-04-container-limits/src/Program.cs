// Mystery service for Lab L08-04: allocation-heavy, with a moderate live set. Behaviour depends on the memory limit and GC settings.
using System.Diagnostics;
int seconds = args.Contains("--seconds") ? int.Parse(args[Array.IndexOf(args, "--seconds") + 1]) : 60;
int liveMB = args.Contains("--live-mb") ? int.Parse(args[Array.IndexOf(args, "--live-mb") + 1]) : 150;
Console.WriteLine($"service pid={Environment.ProcessId} live set {liveMB} MB, running {seconds}s. cpus={Environment.ProcessorCount} serverGC={System.Runtime.GCSettings.IsServerGC}");
var live = new List<byte[]>();
for (int i = 0; i < liveMB; i++) live.Add(new byte[1_000_000]);
var until = Stopwatch.GetTimestamp() + seconds * Stopwatch.Frequency;
long work = 0; var lastReport = Stopwatch.GetTimestamp();
var threads = Enumerable.Range(0, 4).Select(_ => new Thread(() =>
{
    long local = 0;
    while (Stopwatch.GetTimestamp() < until) { var b = new byte[64_000]; b[0] = 1; local += b[0]; }   // short-lived garbage at a high rate
    Interlocked.Add(ref work, local);
})).ToList();
threads.ForEach(t => t.Start());
while (Stopwatch.GetTimestamp() < until)
{
    Thread.Sleep(2000);
    var info = GC.GetGCMemoryInfo();
    Console.WriteLine($"  gc0={GC.CollectionCount(0)} gc1={GC.CollectionCount(1)} gc2={GC.CollectionCount(2)} heap={info.HeapSizeBytes / 1_000_000} MB, limit={info.TotalAvailableMemoryBytes / 1_000_000} MB");
}
threads.ForEach(t => t.Join());
Console.WriteLine($"done: allocations={work}");
GC.KeepAlive(live);
