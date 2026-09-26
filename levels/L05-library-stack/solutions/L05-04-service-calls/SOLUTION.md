# L05-04 - Solution

## What the profile shows
- **Metric:** `connections` = 300 
- **Sampling:** `HttpClient`/`SocketsHttpHandler` construction and `Socket.Connect` dominate over request work.

## Root cause
A new `HttpClient` (and therefore a new `SocketsHttpHandler` and connection pool) per call. Every request opens a new TCP connection (and TLS session, against HTTPS), and closed sockets linger in `TIME_WAIT`. Under load this becomes **socket exhaustion**.

## Fix
Reuse: a `static`/singleton `HttpClient`, or (in DI apps) `IHttpClientFactory` / typed clients. If the client is long-lived, set `PooledConnectionLifetime` (e.g. 2 minutes) so DNS changes are picked up.

## Take-aways
1. **`HttpClient` is meant to be reused**, not created per request; disposing it does not give you back the port instantly.
2. The symptom at scale: `SocketException: Address already in use` or a pile of `TIME_WAIT`, often only under load.
3. Long-lived clients need `PooledConnectionLifetime` so DNS changes are honoured; `IHttpClientFactory` handles that for you (see Level 12).
4. Count *connections*, not just requests, when you test an outbound client.

## Extra credit
Run `ss -tan | grep -c TIME-WAIT` (Linux) before and after each version.

## Go further
Use `IHttpClientFactory` from a small `ServiceCollection`, and compare `connections`. Then set `PooledConnectionLifetime` very small: what happens to the count?
