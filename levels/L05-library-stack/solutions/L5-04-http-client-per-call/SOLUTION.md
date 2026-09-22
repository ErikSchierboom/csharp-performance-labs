# L5-04 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `connections` ≈ 300 (slow) vs a handful, 1–4 (fix).
- **Profile:** `HttpClient`/`SocketsHttpHandler` construction and `Socket.Connect` dominate over request work.

## Root cause
A new `HttpClient` (and therefore a new `SocketsHttpHandler` and connection pool) per call. Every request opens a new TCP connection (and TLS session, against HTTPS), and closed sockets linger in `TIME_WAIT`. Under load this becomes **socket exhaustion**.

## Fix
Reuse: a `static`/singleton `HttpClient`, or (in DI apps) `IHttpClientFactory` / typed clients. If the client is long-lived, set `PooledConnectionLifetime` (e.g. 2 minutes) so DNS changes are picked up.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | connections |
|---|---|---|---|
| before | ≈ 39 ms | 11.56 MB | 300 |
| after | ≈ 10 ms | 3.13 MB | 4 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **`HttpClient` is meant to be reused**, not created per request; disposing it does not give you back the port instantly.
2. The symptom at scale: `SocketException: Address already in use` or a pile of `TIME_WAIT`, often only under load.
3. Long-lived clients need `PooledConnectionLifetime` so DNS changes are honoured; `IHttpClientFactory` handles that for you (see Level 12).
4. Count *connections*, not just requests, when you test an outbound client.

## Go further
Use `IHttpClientFactory` from a small `ServiceCollection`, and compare `connections`. Then set `PooledConnectionLifetime` very small: what happens to the count?

## Further reading
- [HttpClient guidelines for .NET (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- [Fowler, AspNetCoreDiagnosticScenarios: HttpClientGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/HttpClientGuidance.md)
