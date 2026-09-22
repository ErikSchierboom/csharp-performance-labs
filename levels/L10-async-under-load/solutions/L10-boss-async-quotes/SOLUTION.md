# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three Level 10 defects on one path: a synchronous sleep inside an async handler, `.Result` on an async call, and an unbounded per-request fan-out multiplied by request concurrency.

## Root cause
One defect from three Level 10 exercises.

## Fix
`await` everywhere (no blocked pool threads), and one `SemaphoreSlim` shared by all requests in front of the downstream.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | peakInflight |
|---|---|---|---|---|
| before | ≈ 883 ms | 2.59 MB | ≈ 258 ms | 1000 |
| after | ≈ 540 ms | 3.28 MB | ≈ 155 ms | 40 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** `Thread.Sleep` in an `async` handler = **L10-02**; `.Result` = **L10-01**; unbounded fan-out × concurrency = **L10-03**.
2. Blocking defects starve the pool (visible in pool counters); the fan-out overloads the downstream (visible as in-flight count): different tools found different defects.
3. Which did you fix first, and what did the next profile look like?

## Go further
Add a request deadline (`CancellationToken`) and 503 when the permit wait exceeds it (L10-05).

## Further reading
- Your own LAB-LOG entries for L10-01 to L10-03
- docs/READING-LIST.md, Level 10 list
