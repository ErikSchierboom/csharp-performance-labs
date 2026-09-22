# L3-04 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Harness:** gen2 ≈ 150 per run (one per batch) in the slow version; kept ≈ 0 MB in both. A leak would show kept growing.
- **dotTrace timeline:** blocking GC pauses after every batch.

## Root cause
There is no leak. Each batch creates temporary arrays that become garbage immediately. The 'growth' was uncollected garbage: the GC runs when allocation pressure says to. The `GC.Collect()` 'fix' forces a blocking gen2 collection 150 times per run, which is pure overhead.

## Fix
Remove the forced collection. If memory really must stay lower, fix the *allocation pattern* (pool the large arrays; see L2-03) or configure the GC (e.g. `GCHeapHardLimit`, conserve-memory settings), rather than calling `GC.Collect()`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen2 / run | kept after full GC |
|---|---|---|---|---|
| before | ≈ 6.3 ms | 80.21 MB | 150 | 0.00 MB |
| after | ≈ 8.5 ms | 80.17 MB | 10 | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Growing is not leaking.** A leak is memory that stays *reachable*; check retained size after a forced full GC, or snapshot-diff twice.
2. `GC.Collect()` is almost never the right production fix: it turns a cheap, well-tuned background process into a blocking, full one.
3. Reading a memory graph: a sawtooth that returns to the same baseline is healthy; a baseline that ratchets upward is a leak.
4. The kept-after-GC gate in this harness distinguishes the two cases directly.

## Go further
Rent the arrays from `ArrayPool` (L2-03) and compare gen2 counts and peak working set. Then set `GCHeapHardLimit` via `DOTNET_GCHeapHardLimit` and observe.

## Further reading
- [Fundamentals of garbage collection (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
- [Garbage collector config settings (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector)
- Kokosa et al., Pro .NET Memory Management: 'is it a leak?' troubleshooting chapters
