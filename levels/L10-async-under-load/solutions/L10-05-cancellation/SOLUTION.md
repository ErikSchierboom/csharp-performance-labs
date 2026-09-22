# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `stepsAfterAbort` ≈ 17 × requests in the slow version, ≈ 0 in the fix. **Server:** active-request count stays elevated after clients time out.

## Root cause
The handler never observes `RequestAborted`: every abandoned request runs to completion, consuming threads, database connections and downstream capacity for nobody's benefit.

## Fix
Pass `ctx.RequestAborted` to every awaited operation (`Task.Delay(10, ct)` here; `SaveChangesAsync(ct)`, `SendAsync(req, ct)`, `ReadAsync(ct)` in real code). An `OperationCanceledException` from a client disconnect is normal and is not logged as an error.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | stepsAfterAbort |
|---|---|---|---|
| before | ≈ 556 ms | 3.12 MB | 1767 |
| after | ≈ 556 ms | 3.08 MB | 0 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Cancellation is cooperative: you must plumb the token through.** A missing token on one call breaks the chain.
2. Wasted work under retries is a positive feedback loop; cancellation is part of overload protection.
3. Add timeouts *inside* the server too (linked `CancellationTokenSource` with a deadline) so slow downstreams don't hold requests forever.
4. Accept `CancellationToken ct` in your own async methods and pass it on; analyzers (CA2016) flag missing forwarding.

## Go further
Add a server-side deadline of 100 ms by linking `RequestAborted` with a `CancellationTokenSource(100)`. What status should the client see?

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- Microsoft Learn: Cancellation in managed threads *(title only)*
