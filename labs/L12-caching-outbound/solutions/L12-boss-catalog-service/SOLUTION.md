# L12-boss - Solution

## Root cause
One defect from three Lab 12 exercises.

## Fix
`IHttpClientFactory`; single-flight per key (a shared `Lazy<Task>` map for in-flight loads); a bounded `MemoryCache` (`SizeLimit`, `SetSize`).

## Take-aways
1. **Defect -> source:** client per request = **L12-01**; non-atomic get-or-create = **L12-02**; unbounded `MemoryCache` = **L12-03**.
2. Each has its own metric (connections, downstream calls, retained memory): three different tools, one endpoint.
3. The long tail of one-off ids is what makes an unbounded cache dangerous, while the hot ids re-expiring is what makes the stampede repeat instead of being a one-off startup cost.

## Extra credit
What does a `SizeLimit` of 15 (below the hot set of 20) do to the connection count?

## Go further
Replace the hand-built single-flight with `HybridCache` and compare.
