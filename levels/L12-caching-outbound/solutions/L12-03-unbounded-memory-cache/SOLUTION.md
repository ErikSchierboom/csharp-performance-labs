# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Retained:** thousands of strings rooted from the cache's entry table.

## Root cause
A cache with no size limit or expiry keyed by high-cardinality input: a memory leak by design (L3-02 in a real host).

## Fix
`SizeLimit` on the cache, `SetSize` on each entry, and an expiration. Monitor hit rate and eviction count.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | kept after full GC |
|---|---|---|---|---|
| before | ≈ 25 ms | 66.40 MB | ≈ 0 ms | 23.54 MB |
| after | ≈ 27 ms | 67.00 MB | ≈ 0 ms | 3.75 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Every cache needs a bound and an expiry**; choose them from memory budget and access pattern.
2. High-cardinality keys (search text, user ids, URLs) are the danger.
3. `SizeLimit` without `SetSize` throws: the framework forces you to think about entry cost.
4. Track `hit rate`: a bound that's too small turns the cache into pure overhead.

## Go further
Replace with `HybridCache` and configure `MaximumPayloadBytes`. Where does the bound live now?

## Further reading
- [Overview of caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)
- [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- Microsoft Learn: Cache in-memory in ASP.NET Core (size limit) *(title only)*
