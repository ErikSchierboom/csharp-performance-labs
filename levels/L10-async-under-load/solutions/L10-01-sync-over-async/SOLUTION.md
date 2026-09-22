# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Counters:** thread-pool queue length > 0 and thread count growing, CPU near idle. **Timeline:** threads in `Task.Wait`/`GetResult`.

## Root cause
`.Result` blocks a pool thread for the duration of each call, exhausting the pool under concurrency (thread-pool starvation).

## Fix
`async`/`await` in the handler; minimal APIs and MVC both await your `Task`, so nothing blocks.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 573 ms | 2.38 MB | ≈ 284 ms |
| after | ≈ 100 ms | 2.41 MB | ≈ 27 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **The web server's throughput is limited by *blocked threads*, not CPU.** Async handlers scale with concurrency; blocking handlers scale with thread count.
2. Sync-over-async in one hot endpoint can starve the *whole* server, including unrelated endpoints (health checks time out too).
3. Same signature as L4-01: idle CPU, queueing, growing thread count: read the counters, not the CPU graph.
4. Don't fix with `SetMinThreads`; fix the blocking.

## Go further
Add a second endpoint `/health` that returns immediately. Measure its p99 while the slow endpoint is under load, in both versions.

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
