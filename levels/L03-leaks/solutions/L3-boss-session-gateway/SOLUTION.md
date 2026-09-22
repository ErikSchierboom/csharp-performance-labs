# L3-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three independent retention roots: a long-lived event's delegate list, an unbounded profile dictionary, and a callback list whose closures capture a scratch array.

## Root cause
One defect from three Level 3 exercises.

## Fix
Unsubscribe (`IDisposable`); bound the profile cache (oldest-first eviction, well above the 50-entry lookback); capture only the needed value in the callback.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 7.4 ms | 23.69 MB | 23.42 MB |
| after | ≈ 2.4 ms | 23.59 MB | 0.74 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** views never unsubscribing from `Presence` = **L3-01**; an ever-growing `Dictionary` = **L3-02**; closures capturing a big local = **L3-06**.
2. In a snapshot compare, each root has its own retention path: did you find all three by looking at *paths*, not just sizes?
3. Which of the three was largest? Which was hardest to spot?

## Go further
Register the callbacks with an unsubscribe token and remove them when the session ends.

## Further reading
- Your own LAB-LOG entries for L3-01, L3-02, L3-06
- docs/READING-LIST.md, Level 3 section
