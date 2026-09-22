# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `loads` ≈ dozens (slow) vs exactly 5 (fix).

## Root cause
Non-atomic get-or-create in a hot cache: every concurrent miss recomputes the same value (thundering herd).

## Fix
Single-flight per key with `Lazy<Task<T>>` in a `ConcurrentDictionary` (as here), or `HybridCache.GetOrCreateAsync`. Add expiry and decide what happens on failure (don't cache a faulted task forever).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | loads |
|---|---|---|---|---|
| before | ≈ 56 ms | 0.60 MB | ≈ 52 ms | 40 |
| after | ≈ 57 ms | 0.55 MB | ≈ 52 ms | 5 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **A cache miss under load is a synchronised event.** Warm caches before taking traffic, and protect the loader.
2. This is the seed of many incidents: cold cache + burst → backend overload → timeouts → retries → more load.
3. `HybridCache` gives stampede protection and L1+L2 caching; prefer it over hand-rolled code.

## Go further
Try `HybridCache` (needs the `Microsoft.Extensions.Caching.Hybrid` package) and compare `loads`.

## Further reading
- [Overview of caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)
- [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
