# L4-03 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `peakInflight` ≈ 1,000 in the slow version, 50 in the fix.
- **Timeline:** thousands of continuations completing in a burst; per-call latency rises with in-flight count.

## Root cause
Unbounded fan-out. Every item's call is started immediately, so the downstream sees 1,000 concurrent requests and (in this model) responds slowly to all of them. Real systems may time out, shed load or fall over, which triggers retries and makes it worse.

## Fix
Bound the concurrency: `Parallel.ForEachAsync` with `MaxDegreeOfParallelism = 50` (or a `SemaphoreSlim`, or a `Channel` with N consumers). Choose the limit from the downstream's capacity, not from your own core count.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | peakInflight |
|---|---|---|---|
| before | ≈ 506 ms | 0.29 MB | 1000 |
| after | ≈ 125 ms | 0.41 MB | 50 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Unbounded parallelism is not free**: it moves the queue from your code into the downstream (and into memory).
2. Pick the limit from measurement: raise it until throughput stops improving or latency starts climbing.
3. `Task.WhenAll` over a large sequence is a smell; so is `Task.Run` in a loop. Use a bounded primitive.
4. Bounded concurrency also bounds memory (fewer live tasks and buffers).

## Go further
Sweep `MaxDegreeOfParallelism` from 5 to 500 and plot total time. Where's the knee? What would a different downstream (`n*n/500`) change?

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- Microsoft Learn: Parallel.ForEachAsync API docs *(title only)*
