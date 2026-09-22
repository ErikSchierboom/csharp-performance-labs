# L10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** pool threads in `Thread.Sleep` (or the sync API's wait). **Counters:** queue length rising with idle CPU.

## Root cause
A synchronous blocking call inside an `async` method. The method is async in name only; the thread is held for the whole 15 ms.

## Fix
Use the asynchronous API (`await Task.Delay` here; `ReadAllTextAsync`, `ExecuteReaderAsync`, `SendAsync` in real code).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 449 ms | 2.36 MB | ≈ 186 ms |
| after | ≈ 69 ms | 2.48 MB | ≈ 19 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **`async` on the signature guarantees nothing.** Look for synchronous I/O (files, DB drivers, `Send`, `Stream.Read`), `Thread.Sleep`, `.Wait()`, `.Result`, and `lock` around slow work.
2. Analyzers help (e.g. `CA1849` call async methods in async methods; banned-API analyzers for `Thread.Sleep`).
3. Blocking on the thread pool degrades the whole process, not just one endpoint.
4. Prefer libraries with true async I/O; wrapping sync calls in `Task.Run` only moves the blocking to another pool thread.

## Go further
Wrap the sync call in `Task.Run` in a scratch copy. Does it help? What does it cost?

## Further reading
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Debug ThreadPool starvation (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
