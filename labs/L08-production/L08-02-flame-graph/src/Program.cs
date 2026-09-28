// Mystery service for Lab L08-02. Find the hot path from a flame graph, not from reading this file.
using System.Diagnostics;
using System.Runtime.CompilerServices;

var seconds = args.Contains("--seconds") ? int.Parse(args[Array.IndexOf(args, "--seconds") + 1]) : 600;
Console.WriteLine($"service pid={Environment.ProcessId} running for up to {seconds}s (Ctrl-C to stop)");
var until = Stopwatch.GetTimestamp() + seconds * Stopwatch.Frequency;
long sink = 0;
var n = 0;
while (Stopwatch.GetTimestamp() < until)
{
    sink += HandleRequest(n++);
}
GC.KeepAlive(sink);

[MethodImpl(MethodImplOptions.NoInlining)] static long HandleRequest(int i) => Authenticate(i) + Route(i) + Render(i);

[MethodImpl(MethodImplOptions.NoInlining)] static long Authenticate(int i) => Hash(i, 200) + Hash(i + 1, 200);
[MethodImpl(MethodImplOptions.NoInlining)] static long Route(int i) => i % 3 == 0 ? Lookup(i) : Lookup(i + 1);

[MethodImpl(MethodImplOptions.NoInlining)] static long Lookup(int i) => Normalize(i) + Hash(i, 150);
[MethodImpl(MethodImplOptions.NoInlining)] static long Normalize(int i) => Hash(i, 100);

[MethodImpl(MethodImplOptions.NoInlining)] static long Render(int i) => i % 5 == 0 ? Template(i) : Hash(i, 100);
[MethodImpl(MethodImplOptions.NoInlining)] static long Template(int i) => Layout(i) + Layout(i + 1);
[MethodImpl(MethodImplOptions.NoInlining)] static long Layout(int i) => Widgets(i) + Hash(i, 100);
[MethodImpl(MethodImplOptions.NoInlining)] static long Widgets(int i) => Serialize(i) + Serialize(i + 7);
[MethodImpl(MethodImplOptions.NoInlining)] static long Serialize(int i) => Compress(i) + Hash(i, 100);

// The real culprit: reached only through Render -> Template -> Layout -> Widgets -> Serialize -> Compress
[MethodImpl(MethodImplOptions.NoInlining)] static long Compress(int i) => Hash(i, 12_000);

[MethodImpl(MethodImplOptions.NoInlining)] static long Hash(int seed, int rounds)
{
    uint h = (uint)seed * 2654435761u;
    for (int r = 0; r < rounds; r++) h = (h ^ (h >> 13)) * 0x5bd1e995u + (uint)r;
    return h;
}
