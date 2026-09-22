# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocation by type:** controller activation, `ActionContext`/filter pipeline objects, formatter and result objects per request in the MVC version; a small handful in the minimal endpoint.

## Root cause
Pipeline features you don't use still run: MVC's controller activation, filter pipeline and JSON output formatting are per-request work with per-request allocation.

## Fix
A minimal endpoint returning a fixed result (`Results.Text` here). Don't take this as 'never use MVC': use it to **calibrate** what a request costs, then judge each real endpoint against that floor.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 30 ms | 19.31 MB | ≈ 1 ms |
| after | ≈ 18 ms | 11.18 MB | ≈ 0 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Know the floor.** Measure the empty endpoint first; every extra byte per request above it belongs to your code or a framework feature you chose.
2. Frameworks have a per-request tax proportional to the features in the pipeline; it's a budget line, not a bug.
3. Measure per request (total ÷ requests), not per run.
4. A harness that includes the client understates the server's share: keep that in mind when you read the numbers.

## Go further
Add response compression, CORS, auth (a dummy scheme) and logging middleware one at a time. What does each add per request?

## Further reading
- [ASP.NET Core Best Practices (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [Memory management and patterns in ASP.NET Core (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)
- [Conroy, Performance Improvements in ASP.NET Core 8](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)
