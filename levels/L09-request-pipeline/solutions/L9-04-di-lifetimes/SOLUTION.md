# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocations:** a 20,000-entry `Dictionary<int,int>` per request (≈ 500 KB), `ServiceProvider`/`ServiceCollection` internals.
- **Call count:** the `PriceCalculator` constructor runs once per request.

## Root cause
A new DI container per request plus a transient registration of an expensive-to-construct type: the reference data is rebuilt for every request.

## Fix
Register `PriceCalculator` as a **singleton** in the host's container (built once, thread-safe because it's read-only) and let the endpoint receive it as a parameter.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 468 ms | 615.89 MB | ≈ 32 ms |
| after | ≈ 10 ms | 4.26 MB | ≈ 0 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Never build a `ServiceProvider` on the request path** (`BuildServiceProvider` in a handler, or the service-locator pattern with new containers).
2. Match lifetime to cost and state: expensive and immutable → singleton; cheap and stateless → transient; per-request state → scoped.
3. Beware *captive dependencies*: a singleton must not hold a scoped/transient dependency that should be short-lived.
4. The DI container is fast when used as designed; the cost here was construction, not resolution.

## Go further
Make the calculator scoped instead. What changes per request, and why is that still expensive? Then look at `ValidateScopes`/`ValidateOnBuild` for catching lifetime mistakes.

## Further reading
- [ASP.NET Core Best Practices (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [Memory management and patterns in ASP.NET Core (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)
- [Conroy, Performance Improvements in ASP.NET Core 8](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)
- Microsoft Learn: Dependency injection in ASP.NET Core / .NET (service lifetimes) *(title only)*
