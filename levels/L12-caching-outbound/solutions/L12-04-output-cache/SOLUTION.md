# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Handler invocations:** 1,200 (slow) vs ~20 (fix).

## Root cause
Identical, cacheable responses recomputed on every request.

## Fix
`AddOutputCache` + `UseOutputCache` + `.CacheOutput()` on the endpoint (with a sensible policy: expiration, vary-by, tags for invalidation).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 337 ms | 3.63 MB | ≈ 10 ms |
| after | ≈ 7.8 ms | 5.24 MB | ≈ 0 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **The fastest request is one you don't run.** Cache at the highest layer that's correct.
2. Decide the invalidation story (expiry, tags, events) before you cache.
3. Vary on what changes the response (query, headers, user); an under-specified cache key serves wrong data.
4. Output caching helps only for responses that are safe to share; don't cache per-user or sensitive data without vary/authorisation.

## Go further
Add a policy with a 1-second expiration and a tag; evict by tag on a `POST`. What does p99 do at the expiry boundary?

## Further reading
- [Overview of caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)
- [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- [Output caching middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-10.0)
