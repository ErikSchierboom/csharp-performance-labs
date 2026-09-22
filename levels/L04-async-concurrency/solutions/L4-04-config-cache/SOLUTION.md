# L4-04 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Metric:** `factoryCalls` ≈ 20 × (up to 8) in the slow version; exactly 20 in the fix.
- **CPU time:** ~8× the fix, since each round's 8 threads all burn 5 ms.

## Root cause
`ConcurrentDictionary.GetOrAdd(key, Func)` does not lock while running the factory. Threads that miss simultaneously each run it; only the first result is stored. For expensive or side-effecting factories that's redundant work (and, with side effects, potentially wrong behaviour).

## Fix
Store `Lazy<T>` values: `GetOrAdd(key, k => new Lazy<T>(() => Load(k))).Value`. Several threads may create a `Lazy` (cheap), but the default thread-safety mode guarantees the loader runs once. (Async version: cache a `Lazy<Task<T>>` or use `HybridCache`.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | CPU time | factoryCalls |
|---|---|---|---|---|
| before | ≈ 107 ms | 0.09 MB | ≈ 808 ms | 160 |
| after | ≈ 103 ms | 0.09 MB | ≈ 111 ms | 20 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **`GetOrAdd` is not 'compute once'.** Only the *stored value* is unique.
2. Wasted work often shows in CPU and load on dependencies, not in latency; measure both.
3. `Lazy<T>` modes: `ExecutionAndPublication` (default, runs once), `PublicationOnly` (may run many), `None` (not thread-safe).
4. This is the seed of a **cache stampede** (Level 12): many misses recompute the same expensive value at once.

## Go further
Make the loader `async` and cache `Lazy<Task<int>>`. What happens if the task faults: is the failure cached forever?

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- Microsoft Learn: Lazy<T> and ConcurrentDictionary.GetOrAdd remarks *(title only)*
