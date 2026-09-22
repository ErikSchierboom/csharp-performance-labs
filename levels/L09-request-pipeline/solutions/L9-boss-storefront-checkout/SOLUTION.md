# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Four independent L9 defects on one request path.

## Root cause
One defect from four Level 9 exercises.

## Fix
Static `Regex` and `[LoggerMessage]` in the middleware; the tax table as a real singleton; deserialise straight from the body stream; build the receipt once and write it once.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen2 / run | p99 latency |
|---|---|---|---|---|
| before | ≈ 443 ms | 952.71 MB | 19 | ≈ 28 ms |
| after | ≈ 310 ms | 300.43 MB | 0 | ≈ 17 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** per-request `Regex` and interpolated log = **L9-02**; `BuildServiceProvider` per request = **L9-04**; body → string = **L9-05**; flush per line = **L9-03**.
2. Each layer of the pipeline had its own signature in the allocation view: which types pointed at which?
3. Measure bytes per request before and after each fix: which gave the largest drop?

## Go further
Compare the result with the L9-01 floor: how many bytes per request are left above an empty endpoint?

## Further reading
- Your own LAB-LOG entries for L9-02 to L9-05
- docs/READING-LIST.md, Level 9 list
