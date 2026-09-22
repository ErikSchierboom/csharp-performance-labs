# L11 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **SQL:** one query with two joins returning 400 rows; after the fix, three queries returning 1 + 20 + 20 rows.

## Root cause
Two collection navigations in one JOIN multiply rows (Cartesian product), inflating data transfer, materialisation and memory.

## Fix
`AsSplitQuery()` (extra round trips but far less data), or project to a DTO with aggregates. Split queries are not free: consider consistency and round trips against a networked database.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 147 ms | 122.18 MB | ≈ 10 ms |
| after | ≈ 30 ms | 29.10 MB | ≈ 4 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Multiple collection Includes multiply rows.** EF warns about it for a reason.
2. Compare rows returned, not just query count.
3. Projection (`Select`) usually beats `Include` for read models.

## Go further
Project to `{ Total = c.Orders.Sum(...), Addresses = c.Addresses.Count() }` and compare with split queries.

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Fowler, AspNetCoreDiagnosticScenarios](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios)
