# L12-01 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Metric:** `connections` ≈ one per request (slow) vs a handful (fix).

## Root cause
A new `HttpClient`/handler (and connection pool) per request defeats connection reuse.

## Fix
`IHttpClientFactory` (`AddHttpClient`) and `factory.CreateClient()`: handlers are pooled and rotated on a schedule. (A singleton `HttpClient` with `PooledConnectionLifetime` is the alternative.)

## Take-aways
1. **Reuse the handler, not the request.** This is L05-04 on the request path, where load multiplies it.
2. Factory-managed clients avoid both socket exhaustion and stale DNS.
3. Typed clients + resilience handlers (timeouts, retries) hang naturally off the factory.

## Extra credit
Use a `static readonly HttpClient` without `PooledConnectionLifetime`. What breaks when the callee's DNS changes?

## Go further
Add a typed client and a `Microsoft.Extensions.Http.Resilience` pipeline. What changes in `connections`?
