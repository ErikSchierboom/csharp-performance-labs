# L13-01 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Metric:** `connections` ≈ requests (slow) vs a handful (fix).

## Root cause
A `Connection: close` header disables keep-alive: one TCP connection per request.

## Fix
Remove the header. In real deployments verify keep-alive end to end: client pool, load balancer idle timeout vs Kestrel `KeepAliveTimeout`, and HTTP/2 where available.

## Take-aways
1. **Connection setup can dominate small requests.** Watch connections per request, not just latency.
2. Config copied from snippets ships bugs: review middleware and proxy config as code.
3. Timeouts must nest correctly: client < LB idle timeout < server keep-alive (or you get resets on reused connections).

## Extra credit
Set `KeepAliveTimeout` to 1 ms instead. What happens to `connections`?

## Go further
Enable HTTP/2 in Kestrel and compare connections and latency for 16 concurrent users.
