# L6-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three separate hardware/runtime effects; the profile is three flat, unremarkable loops.

## Root cause
One defect from each of three Level 6 exercises.

## Fix
Walk the grid in memory order; make `Reading` a struct so the array holds the data inline; use the vectorised `Count` on a span.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 29 ms | 0.00 MB |
| after | ≈ 6.1 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** column-major walk = **L6-01**; array of class instances (pointer chasing) = **L6-02**; LINQ `Count` with a lambda = **L6-08**.
2. None of these show as a slow *function*: they show as flat loops. Predict-then-measure is the only way in.

## Go further
Which of the three is limited by memory bandwidth after the fix, and which by instruction throughput? How would you tell (`perf stat`)?

## Further reading
- Your own LAB-LOG entries for L6-01, L6-02, L6-08
- docs/BENCHMARKDOTNET.md
- docs/READING-LIST.md, Level 6 section
