# L7-03 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Harness:** `committedMB` ≈ 300 (slow) vs ≈ 60 (fix); gen0 collection count is tiny in the slow version (big budget, few collections).
- **runtimeconfig:** `System.GC.Server: true`, `System.GC.DynamicAdaptationMode: 0`.

## Root cause
Server GC with dynamic adaptation opted out and no heap-count cap, on a machine with many cores. The GC sizes itself for a big, busy server: many heaps, each with a large gen0 budget, so committed memory balloons even though live data is tiny.

## Fix
Cap the heaps to what the workload needs: `System.GC.HeapCount = 4` (the exercise's runtime option; also `DOTNET_GCHeapCount`). Alternatives: leave dynamic adaptation on (the default for Server GC from .NET 9, which shrinks the heap count to fit the app), or use Workstation GC for small services, or set `GCHeapHardLimit` for containers. Whichever you choose, measure throughput too.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | committedMB | workingSetMB |
|---|---|---|---|---|
| before | ≈ 214 ms | 4632.62 MB | 348 | 386 |
| after | ≈ 185 ms | 4632.60 MB | 65 | 100 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Configuration is part of the program.** Same binary, different GC mode/cores/limits ⇒ different memory and latency.
2. Server GC trades memory for throughput; a small service on a big node rarely needs 20 heaps.
3. In containers, the runtime reads the cgroup memory/CPU limits; know what it reads and what it defaults to (DATAS on .NET 9+ helps).
4. Always print GC mode, core count and limits at the start of any benchmark or incident report (this harness does).

## Go further
Run the exercise with `DOTNET_gcServer=0` and with `DOTNET_GCHeapCount=1..8` and plot committed memory and time. Where is your knee?

## Further reading
- [Garbage collector config settings (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector)
- [Stephens, Preparing for the .NET 10 GC (DATAS)](https://devblogs.microsoft.com/dotnet/preparing-for-dotnet-10-gc/)
- [Collect diagnostics in Linux containers (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/diagnostics-in-containers)
