# L3-05 · Tag registry

## Symptom
A helper attaches per-object metadata (view counts, labels) to documents through a lookup table. Documents are created, tagged, used and dropped, but after 20,000 of them about **90 MB stays reachable**. Nothing in the app holds the documents; only the tagging helper touched them.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 28 ref-ms |
| Median allocated | 205 MB |
| Kept after a full GC | ≤ 1 MB |

## Note
This exercise has a **kept-after-a-full-GC** budget (the leak gate). The harness calls `Workload.Reset()` before each run to clear *test scaffolding*; that reset is not the fix.
In your profiler's memory-snapshot view (dotMemory's Compare, VS's Memory Usage diff, or a pair of `dotnet-gcdump` snapshots): take a snapshot, run `--profile --seconds 5`, take another, and diff them, then look at what dominates the type that grew.

## Extra credit
What happens if `Metadata` holds a strong reference back to its `Document` in the `ConditionalWeakTable` version? Try it and look at the kept figure (there is a documented guarantee here).
