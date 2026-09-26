# L03-04 - Solution

## What the profile shows
- **Harness:** gen2 ≈ 150 per run (one per batch) in the slow version; kept ≈ 0 MB in both. A leak would show kept growing.
- **timeline:** blocking GC pauses after every batch.

## Root cause
There is no leak. Each batch creates temporary arrays that become garbage immediately. The 'growth' was uncollected garbage: the GC runs when allocation pressure says to. The `GC.Collect()` 'fix' forces a blocking gen2 collection 150 times per run, which is pure overhead.

## Fix
Remove the forced collection. If memory really must stay lower, fix the *allocation pattern* (pool the large arrays; see L02-03) or configure the GC (e.g. `GCHeapHardLimit`, conserve-memory settings), rather than calling `GC.Collect()`.

## Take-aways
1. **Growing is not leaking.** A leak is memory that stays *reachable*; check retained size after a forced full GC, or snapshot-diff twice.
2. `GC.Collect()` is almost never the right production fix: it turns a cheap, well-tuned background process into a blocking, full one.
3. Reading a memory graph: a sawtooth that returns to the same baseline is healthy; a baseline that ratchets upward is a leak.
4. The kept-after-GC gate in this harness distinguishes the two cases directly.

## Extra credit
Change one batch to *actually* leak (add the `big` array to a static list). What does the kept figure do? Now you have a leak fingerprint to compare against.

## Go further
Rent the arrays from `ArrayPool` (L02-03) and compare gen2 counts and peak working set. Then set `GCHeapHardLimit` via `DOTNET_GCHeapHardLimit` and observe.
