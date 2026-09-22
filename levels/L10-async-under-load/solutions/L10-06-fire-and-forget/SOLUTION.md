# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `peakConcurrentJobs` in the hundreds (slow) vs 4 (fix). **Counters:** thread count and queue length spike during the burst; HTTP p99 rises.

## Root cause
Unbounded fire-and-forget (`Task.Run` per request) removes all back-pressure: background concurrency equals request concurrency, and blocking jobs compete with request handling for the thread pool.

## Fix
A bounded `Channel<T>` feeding N long-running workers. The endpoint enqueues (awaiting if the queue is full) and returns 202. In production use a `BackgroundService`, decide what a full queue means (429/503 vs waiting), and persist jobs if they must survive restarts.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | peakConcurrentJobs |
|---|---|---|---|---|
| before | ≈ 84 ms | 0.83 MB | ≈ 39 ms | 21 |
| after | ≈ 383 ms | 0.81 MB | ≈ 1 ms | 4 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Fire-and-forget is unbounded work by another name.** Anything you can't bound is an outage waiting for a burst.
2. Background work shares the process's resources; isolate it (dedicated workers, bounded parallelism, separate pool if needed).
3. Decide the failure policy for a full queue up front: wait, reject, drop oldest, or spill to durable storage.
4. Unobserved exceptions in `Task.Run` are silently lost unless handled: another reason to use a supervised worker.

## Go further
Return 429 immediately when the queue is full (`TryWrite`) instead of awaiting. What does the client see, and how would it retry?

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- Microsoft Learn: Background tasks with hosted services in ASP.NET Core *(title only)*
