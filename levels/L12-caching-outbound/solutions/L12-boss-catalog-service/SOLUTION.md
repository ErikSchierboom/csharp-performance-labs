# L12-boss - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- Three Level 12 defects: a `new HttpClient()` per load, a non-atomic cache that lets concurrent misses all load, and a `MemoryCache` with no size limit or expiry.

## Root cause
One defect from three Level 12 exercises.

## Fix
`IHttpClientFactory`; single-flight per key (a shared `Lazy<Task>` map for in-flight loads); a bounded `MemoryCache` (`SizeLimit`, `SetSize`, sliding expiry).

## Take-aways
1. **Defect → source:** client per request = **L12-01**; non-atomic get-or-create = **L12-02**; unbounded `MemoryCache` = **L12-03**.
2. Each has its own metric (connections, loads per key, retained memory): three different tools, one endpoint.
3. The long tail of one-off ids is what makes an unbounded cache dangerous, while the hot ids are what makes the stampede visible.

## Extra credit
What does a `SizeLimit` of 40 (below the hot set of 60) do to the connection count?

## Go further
Replace the hand-built single-flight with `HybridCache` and compare.
