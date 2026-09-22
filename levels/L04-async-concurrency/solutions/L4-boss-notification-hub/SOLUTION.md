# L4-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three independent defects in three stages.

## Root cause
One defect from each of three earlier levels, in three separate stages of a single batch.

## Fix
Unsubscribe inboxes (dispose); do the pure work outside the lock and use an atomic add; bound the queue.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | maxQueued | kept after full GC |
|---|---|---|---|---|
| before | ≈ 97 ms | 14.49 MB | 1887 | 4.65 MB |
| after | ≈ 29 ms | 14.41 MB | 50 | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** inboxes never unsubscribe from a long-lived bus = **L3-01**; expensive work inside a `lock` = **L4-02**; unbounded producer/consumer queue = **L4-05**.
2. Final bosses test *recognition*: which signature (retained memory, contention/idle threads, queue depth) pointed at which stage?

## Go further
Order the three by how much each contributes to the time budget, then to memory. Are they different orders?

## Further reading
- Your own LAB-LOG entries for L3-01, L4-02, L4-05
- docs/READING-LIST.md, Level 3–4 sections
