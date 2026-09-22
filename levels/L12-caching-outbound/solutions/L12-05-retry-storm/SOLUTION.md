# L12 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metrics:** `downstreamCalls` ≈ 2–3× requests, `giveUps` > 0 (slow); ≈ 1× and 0 (fix).

## Root cause
Immediate, unbounded-rate retries against an overloaded dependency amplify the load that caused the failures (positive feedback).

## Fix
Bound concurrency toward the dependency (`SemaphoreSlim(15)`, below the downstream's capacity of 20), so it never overloads; then retries are rarely needed. In production add timeouts, capped retries with jittered exponential backoff, and a circuit breaker (`Microsoft.Extensions.Http.Resilience`).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | downstreamCalls | giveUps |
|---|---|---|---|---|---|
| before | ≈ 54 ms | 1.45 MB | ≈ 31 ms | 1160 | 380 |
| after | ≈ 820 ms | 1.30 MB | ≈ 183 ms | 400 | 0 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Retries are load.** Without limits, they turn blips into outages.
2. Prefer prevention (concurrency limits, load shedding) to reaction (retries).
3. If you retry: cap attempts, add exponential backoff with jitter, and use a retry *budget* (e.g. ≤ 10% extra calls).
4. Make retried operations idempotent, and propagate deadlines.

## Go further
Add jittered backoff to the *slow* version. How far does that get without a bulkhead?

## Further reading
- [Overview of caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)
- [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- Nygard, Release It! (circuit breakers, bulkheads) *(title only)*
- Brooker, Timeouts, retries and backoff with jitter (Amazon Builders' Library) *(title only)*
