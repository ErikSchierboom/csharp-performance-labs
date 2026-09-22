# L11 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `sqlCommands` ≈ 21 per request (slow) vs 1 (fix).

## Root cause
A per-customer query inside a loop: N+1 on the request path, multiplying database work by the fan-out.

## Fix
One projection with a server-side aggregate (`Select(c => c.Orders.Sum(...))`), `AsNoTracking`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | sqlCommands |
|---|---|---|---|---|
| before | ≈ 284 ms | 150.72 MB | ≈ 21 ms | 8400 |
| after | ≈ 23 ms | 24.80 MB | ≈ 3 ms | 400 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Statements per request** is the metric that exposes N+1 in production; alert on it.
2. Concurrency turns a wasteful query pattern into database saturation.
3. Find it from traces before reading code (the ASP.NET Core levels (9–14) checkpoint).

## Go further
Add an OpenTelemetry/EF logging listener that counts statements per request and fails a test above 3.

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Fowler, AspNetCoreDiagnosticScenarios](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios)
