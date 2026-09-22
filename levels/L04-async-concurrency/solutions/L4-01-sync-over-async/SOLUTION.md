# L4-01 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** pool threads mostly *Waiting* (in `Task.Wait`/`GetResult`), thread count creeping upward, CPU ≈ 0.
- **`dotnet-counters`:** `threadpool-queue-length` > 0 while CPU is low: the starvation signature.
- **Sampling** would show almost nothing: threads that are blocked are not on-CPU, so a CPU profile can't see the problem.

## Root cause
`Handle` blocks a pool thread with `.Result` on a task that needs *another* pool thread to complete. Under a burst, blocked threads exhaust the pool, the runtime injects new threads gradually, and requests queue behind each other: **thread-pool starvation**. Latency grows with the burst size; CPU stays idle.

## Fix
Go async all the way: `HandleAsync` awaits the repository call, so no thread is held during the 20 ms. The only remaining blocking wait is at the very edge (the harness); in a web app the framework awaits your `Task` for you.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 485 ms | 0.12 MB | ≈ 468 ms |
| after | ≈ 21 ms | 0.09 MB | ≈ 21 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Slow but idle CPU ⇒ suspect waiting**, and sampling can't see waiting: use the timeline and pool counters.
2. Sync-over-async also risks outright deadlocks where a synchronization context exists (UI, old ASP.NET).
3. Fixing one blocking call isn't enough if a caller further up blocks on your `Task`: async has to reach the top of the call chain.
4. Don't 'fix' starvation with `SetMinThreads` in production; it hides the blocking and costs memory and context switches.

## Go further
Increase `SetMinThreads` to 200 in a scratch copy: it 'works'. Compare thread count and memory with the async version, and say why that's a bad trade.

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Gosse, .NET ThreadPool starvation, and how queuing makes it worse](https://medium.com/criteo-engineering/net-threadpool-starvation-and-how-queuing-makes-it-worse-512c8d570527)
