# L11 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Pool:** 8/8 in use, many waiters; each connection spends ~97% of its hold time idle, waiting on the third party.

## Root cause
Connections are held across an unrelated slow call, so pool capacity ÷ hold time caps throughput far below what the database could serve.

## Fix
Reorder so the connection is rented after the slow call and released immediately after the query; keep rent scopes as small as possible.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 2518 ms | 2.20 MB | ≈ 254 ms |
| after | ≈ 329 ms | 2.16 MB | ≈ 40 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Hold scarce resources briefly.** Connections, locks and semaphores should wrap only the work that needs them.
2. Pool exhaustion is a throughput ceiling (size ÷ hold time), not necessarily a database problem.
3. Also seen with transactions held across HTTP calls or user think-time.
4. The same reasoning applies to thread-pool threads, `HttpClient` connections and semaphore permits.

## Go further
Add a timeout to `RentAsync` and return 503 quickly when the pool is exhausted. What do you gain and lose?

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Fowler, AspNetCoreDiagnosticScenarios](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios)
