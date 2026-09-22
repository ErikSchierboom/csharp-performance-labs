# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocations:** `Regex` internals (parser, code), `Match`/`Group`, interpolated `string`, header strings, per request.

## Root cause
Loop-invariant work done per request (a `new Regex` in the pipeline) plus eagerly formatted log messages for a disabled level.

## Fix
A `static readonly Regex` (compiled or `[GeneratedRegex]`) and a `[LoggerMessage]` source-generated log method. Middleware runs on **every** request, so its per-request cost is multiplied by your entire traffic.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 36 ms | 31.67 MB | ≈ 1 ms |
| after | ≈ 28 ms | 15.63 MB | ≈ 1 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Middleware is on every request's critical path.** A microsecond there is a CPU-second per million requests.
2. Everything you learned about allocation (Levels 1, 2, 5) applies unchanged; the request rate is what multiplies it.
3. Hoist anything that doesn't depend on the request to a static or singleton.
4. Measure per request before and after; budgets on bytes/request catch this class of regression.

## Go further
Use `[GeneratedRegex]` instead of `RegexOptions.Compiled` and compare. Then replace the regex entirely with `PathString`/span parsing.

## Further reading
- [ASP.NET Core Best Practices (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [Memory management and patterns in ASP.NET Core (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)
- [Conroy, Performance Improvements in ASP.NET Core 8](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)
