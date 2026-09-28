# L07-03 - Solution

## What the profile shows
- **Harness:** `committedMB` is about 16 MB per core: 33 (2 cores), 65 (4), 352 (20). The fix is 3 MB on all three.
- **GC:** one heap per core, and only 15 gen0 collections per run (the fix: 1,880), because each heap has a large allocation budget. `GC.GetConfigurationVariables()["HeapCount"]` shows the heap count the GC was configured with, which is also the *maximum* when dynamic adaptation is on, not the number in use.

## Root cause
Server GC with dynamic adaptation opted out and no heap-count cap, on a machine with many cores. The GC sizes itself for a big, busy server: many heaps, each with a large gen0 budget, so committed memory balloons even though live data is tiny.

## Fix
Delete the pasted `System.GC.DynamicAdaptationMode=0` line. Dynamic adaptation (DATAS) is on by default for Server GC from .NET 9, and it sizes the heaps to what the app actually uses.

Workstation GC and a heap count of 1 also pass. Capping the heap count at 4 only helps on machines with more than 4 cores, which is the "works on my laptop" trap again: a fix tuned for one machine. Whichever you choose, measure throughput too.

## Take-aways
1. **Configuration is part of the program.** Same binary, different GC mode/cores/limits ⇒ different memory and latency.
2. Server GC trades memory for throughput; a small service on a big node rarely needs 20 heaps.
3. In containers, the runtime reads the cgroup memory/CPU limits; know what it reads and what it defaults to (DATAS on .NET 9+ helps).
4. Always print GC mode, core count and limits at the start of any benchmark or incident report (this harness does).

## Extra credit
With DATAS back on, gen0 collections go from 15 to 1,880 per run, but the run time barely moves. Why are those collections so cheap here? What would a workload look like that *does* pay for them?

## Go further
Run the exercise with `DOTNET_gcServer=0` and with `DOTNET_GCHeapCount=1..8` and plot committed memory and time. Where is your knee?
