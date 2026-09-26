# L03-01 - Event hub

*Gone but Not Forgotten*

## Symptom
After a batch of 2,000 short-lived widgets, about **20 MB stays reachable** even after a full GC, and each batch gets slower than the last. Each widget was "dropped" as soon as it was used, and nothing else refers to it in the code you can see.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 11 ref-ms |
| Median allocated | 49 MB |
| Kept after a full GC | ≤ 1 MB |

## Note
This exercise has a **kept-after-a-full-GC** budget (the leak gate). The harness calls `Workload.Reset()` before each run to clear *test scaffolding*; that reset is not the fix.
In your profiler's memory-snapshot view (dotMemory's Compare, VS's Memory Usage diff, or a pair of `dotnet-gcdump` snapshots): take a snapshot, run `--profile --seconds 5`, take another, and diff them, then look at what dominates the type that grew.
