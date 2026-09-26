# L11-01 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Metric:** `sqlCommands` ≈ 21 per request (slow) vs 1 (fix).

## Root cause
A per-customer query inside a loop: N+1 on the request path, multiplying database work by the fan-out.

## Fix
One projection with a server-side aggregate (`Select(c => c.Orders.Sum(...))`), `AsNoTracking`.

## Take-aways
1. **Statements per request** is the metric that exposes N+1 in production; alert on it.
2. Concurrency turns a wasteful query pattern into database saturation.
3. Find it from traces before reading code (the ASP.NET Core levels (9–14) checkpoint).

## Extra credit
Load the *orders themselves* with `Include` instead. How does the row count and allocation compare with the projection?

## Go further
Add an OpenTelemetry/EF logging listener that counts statements per request and fails a test above 3.
