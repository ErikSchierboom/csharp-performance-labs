# L3-02 · Session cache

## Symptom
A session lookup cache makes the app fast, but after a 40,000-request batch about **80 MB stays reachable** after a full GC. Traffic in production has no upper bound, and the box's memory
grows steadily until the container is killed. Only *recent* sessions are ever looked up again.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 28 ref-ms |
| Median allocated | 202 MB |
| Kept after a full GC | ≤ 6 MB |

## Note
This exercise has a **kept-after-a-full-GC** budget (the leak gate). The harness calls `Workload.Reset()` before each run to clear *test scaffolding*; that reset is not the fix.
In your profiler's memory-snapshot view (dotMemory's Compare, VS's Memory Usage diff, or a pair of `dotnet-gcdump` snapshots): take a snapshot, run `--profile --seconds 5`, take another, and diff them, then look at what dominates the type that grew.

## Extra credit
Set the capacity to 100 (below the 200-session working set). What happens to the checksum, and what does that tell you about picking a bound?
