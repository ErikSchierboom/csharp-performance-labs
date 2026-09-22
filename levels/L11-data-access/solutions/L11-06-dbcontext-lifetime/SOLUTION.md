# L11 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Retained memory:** thousands of `Product` + snapshots + strings, rooted from the static context's change tracker.

## Root cause
A singleton/static tracking `DbContext` accumulates every loaded entity (and needs a lock because it isn't thread-safe).

## Fix
One short-lived context per request/unit of work (`AddDbContext` scoped, or pooled with `AddDbContextPool`) and `AsNoTracking` for read-only queries.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | kept after full GC |
|---|---|---|---|---|
| before | ≈ 246 ms | 62.67 MB | ≈ 16 ms | 46.29 MB |
| after | ≈ 42 ms | 66.18 MB | ≈ 2 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **`DbContext` = unit of work, short-lived, not thread-safe.** Never register it as a singleton.
2. The change tracker is a cache you didn't ask for: it's why long-lived contexts grow.
3. Locks around a shared context hide a design error and serialise your server.
4. This is L3 (retention) inside an ASP.NET app: the same tools (snapshot, dominators) find it.

## Go further
Register `AddDbContextPool` and compare per-request allocation with `new` contexts.

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Fowler, AspNetCoreDiagnosticScenarios](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios)
