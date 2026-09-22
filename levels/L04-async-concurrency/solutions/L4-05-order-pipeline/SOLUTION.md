# L4-05 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `maxQueued` ≈ thousands (slow) vs ≤ 51 (fix).
- **dotMemory:** live `byte[]` climbs then falls as the consumer drains.

## Root cause
An unbounded queue between mismatched stages. Nothing slows the producer, so the queue length is limited only by memory.

## Fix
`Channel.CreateBounded(capacity)` with `FullMode = Wait`, and `await WriteAsync` in the producer. The producer now runs at the consumer's pace, and memory use is capped at `capacity × item size`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | maxQueued |
|---|---|---|---|
| before | ≈ 25 ms | 28.93 MB | 2797 |
| after | ≈ 26 ms | 28.93 MB | 50 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Every queue needs a bound.** An unbounded queue hides overload until it becomes an outage.
2. Back-pressure propagates: a slow consumer slows the producer, which can in turn slow *its* caller: that's the system telling you about capacity.
3. Pick the bound from memory budget and acceptable latency (queue length × service time = wait). Add load shedding when waiting isn't acceptable.
4. Same idea at every scale: thread-pool queues, message-broker prefetch, HTTP accept queues.

## Go further
Switch `FullMode` to `DropOldest` and explain what you gave up. When is dropping the right call?

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- Microsoft Learn: System.Threading.Channels overview *(title only)*
