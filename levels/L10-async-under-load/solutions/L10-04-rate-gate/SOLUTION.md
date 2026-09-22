# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Counters:** healthy pool, low CPU. **Outbound calls:** ≈ one per request; **queue:** requests waiting on `SemaphoreSlim`.

## Root cause
A global `SemaphoreSlim(1)` serialises a slow, cacheable remote call for every request, so throughput is capped at one call time and every request pays the queue.

## Fix
Cache the fetch per currency as a shared `Lazy<Task<decimal>>` (one call per key, awaited by all). In production add expiry, refresh-ahead and failure handling (`HybridCache` does much of this).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 4072 ms | 1.39 MB | ≈ 512 ms |
| after | ≈ 3.5 ms | 1.12 MB | ≈ 1 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **An async lock removes thread blocking but not serialisation.** A gate on the hot path caps throughput.
2. If the data is shared and slow-changing, **cache it** and dedupe concurrent fetches (single-flight).
3. Decide what happens on failure and on expiry: a cached faulted task must not be cached forever.
4. Lock *refresh*, not *reads*.

## Go further
Add a 5-second expiry with refresh-ahead: serve the stale value while one request refreshes. What can go wrong if the refresh fails?

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Microsoft Learn: HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
