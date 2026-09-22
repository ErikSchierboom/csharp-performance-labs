# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `connections` ≈ one per request (slow) vs a handful (fix).

## Root cause
A new `HttpClient`/handler (and connection pool) per request defeats connection reuse.

## Fix
`IHttpClientFactory` (`AddHttpClient`) and `factory.CreateClient()`: handlers are pooled and rotated on a schedule. (A singleton `HttpClient` with `PooledConnectionLifetime` is the alternative.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | connections |
|---|---|---|---|---|
| before | ≈ 18 ms | 12.43 MB | ≈ 2 ms | 400 |
| after | ≈ 7.0 ms | 2.39 MB | ≈ 1 ms | 16 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Reuse the handler, not the request.** This is L5-04 on the request path, where load multiplies it.
2. Factory-managed clients avoid both socket exhaustion and stale DNS.
3. Typed clients + resilience handlers (timeouts, retries) hang naturally off the factory.

## Go further
Add a typed client and a `Microsoft.Extensions.Http.Resilience` pipeline. What changes in `connections`?

## Further reading
- [Overview of caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)
- [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
