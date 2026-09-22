# L2-04 · Tile cache

## Symptom
Rendering 1.5 million map tiles takes about **300 ms** and allocates **172 MB**, which is reasonable for 1.5M small objects with a
64-byte buffer each. But the harness shows a non-zero **gen1** count, and a similar loop with a differently written class
runs much faster. The allocation budget is deliberately generous here: **the problem is not how many bytes you allocate.**

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 260 ref-ms |
| Median allocated | 200 MB |

## Note
Time is scaled to your machine; allocation and collection counts are not.

## Extra credit
Try three variants and rank them: (a) remove the finalizer; (b) keep the finalizer and call `GC.SuppressFinalize(tile)` when done;
(c) implement `IDisposable` properly. Which is fastest, and why is (b) still slower than (a) even though the finalizer never runs?
