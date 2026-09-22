# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three Level 12 defects: a `new HttpClient()` per load, a non-atomic cache that lets concurrent misses all load, and a `MemoryCache` with no size limit or expiry.

## Root cause
One defect from three Level 12 exercises.

## Fix
`IHttpClientFactory`; single-flight per key (a shared `Lazy<Task>` map for in-flight loads); a bounded `MemoryCache` (`SizeLimit`, `SetSize`, sliding expiry).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | connections | kept after full GC |
|---|---|---|---|---|---|
| before | ≈ 338 ms | 68.43 MB | ≈ 40 ms | 300 | 18.43 MB |
| after | ≈ 331 ms | 60.92 MB | ≈ 34 ms | 32 | 4.84 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** client per request = **L12-01**; non-atomic get-or-create = **L12-02**; unbounded `MemoryCache` = **L12-03**.
2. Each has its own metric (connections, loads per key, retained memory): three different tools, one endpoint.
3. The long tail of one-off ids is what makes an unbounded cache dangerous, while the hot ids are what makes the stampede visible.

## Go further
Replace the hand-built single-flight with `HybridCache` and compare.

## Further reading
- Your own LAB-LOG entries for L12-01 to L12-03
- docs/READING-LIST.md, Level 12 list
