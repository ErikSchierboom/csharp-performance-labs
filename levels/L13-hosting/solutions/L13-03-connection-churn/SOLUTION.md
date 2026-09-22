# L13 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `connections` ≈ requests (slow) vs a handful (fix).

## Root cause
A `Connection: close` header disables keep-alive: one TCP connection per request.

## Fix
Remove the header. In real deployments verify keep-alive end to end: client pool, load balancer idle timeout vs Kestrel `KeepAliveTimeout`, and HTTP/2 where available.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | connections |
|---|---|---|---|---|
| before | ≈ 38 ms | 27.49 MB | ≈ 2 ms | 1200 |
| after | ≈ 8.4 ms | 2.88 MB | ≈ 0 ms | 16 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Connection setup can dominate small requests.** Watch connections per request, not just latency.
2. Config copied from snippets ships bugs: review middleware and proxy config as code.
3. Timeouts must nest correctly: client < LB idle timeout < server keep-alive (or you get resets on reused connections).

## Go further
Enable HTTP/2 in Kestrel and compare connections and latency for 16 concurrent users.

## Further reading
- Kestrel web server in ASP.NET Core (Microsoft Learn) *(title only)*
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [ASP.NET Core built-in metrics](https://learn.microsoft.com/en-us/aspnet/core/metrics/built-in?view=aspnetcore-10.0)
