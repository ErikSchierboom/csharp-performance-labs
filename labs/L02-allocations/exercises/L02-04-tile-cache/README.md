# L02-04 - Tile cache

*Tiles All the Way Down*

## Symptom
Rendering 1.5 million map tiles takes about **400 ms** and allocates **172 MB**, which is reasonable for 1.5M small objects with a
64-byte buffer each. But the harness shows a non-zero **gen1** count for objects that live for a single iteration, and the time is
far more than 1.5M small allocations should cost. The allocation budget is deliberately generous here: **the problem is not how many bytes you allocate.**

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 100 ref-ms |
| Median allocated | 200 MB |

## Note
Time is scaled to your machine; allocation and collection counts are not.