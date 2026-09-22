# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `peakInflight` ≈ 480 (slow) vs ≤ 40 (fix). **Downstream latency:** climbs sharply with in-flight count.

## Root cause
Unbounded fan-out multiplied by concurrent requests overloads the shared downstream, which slows every call: latency is driven by the *product* of the two concurrency levels.

## Fix
A `SemaphoreSlim(40)` shared across requests gates calls into the downstream. Requests queue briefly for permits instead of collectively overwhelming it.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | peakInflight |
|---|---|---|---|---|
| before | ≈ 1054 ms | 1.93 MB | ≈ 121 ms | 480 |
| after | ≈ 619 ms | 2.80 MB | ≈ 64 ms | 40 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Bound concurrency at the shared resource**, not per request: total in-flight = Σ over requests.
2. This is L4-03 at request scale; the same 'sweet spot' logic (measure the knee) applies.
3. A bulkhead per dependency also stops one slow dependency from consuming all your capacity.
4. Also consider a timeout and load shedding: waiting for a permit forever is its own failure mode.

## Go further
Sweep the permit count (5 to 200) and plot p99. Then replace the semaphore with `Parallel.ForEachAsync` per request and explain why it doesn't fix the total.

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- Nygard, Release It! (bulkheads, timeouts) *(title only)*
