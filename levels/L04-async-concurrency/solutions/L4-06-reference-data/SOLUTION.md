# L4-06 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** threads blocked on one monitor; contention count in the millions.
- **Sampling:** time inside `Monitor.Enter`/`TryEnter` slow path (spinning, then waiting).

## Root cause
A single hot lock on a read-dominated path: even a tiny critical section becomes a serialisation point when many threads take it back-to-back (lock convoy, cache-line ping-pong on the lock word).

## Fix
Copy-on-write snapshot: readers read a `volatile` reference to an immutable-by-convention dictionary (lock-free); the rare writer copies, changes, and swaps the reference under a writer-only lock. Cost: each write copies the whole map: fine for 1,000 entries and a few writes; wrong for large maps with frequent writes.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 69 ms | 0.07 MB |
| after | ≈ 8.3 ms | 1.77 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Read-mostly data wants lock-free reads.** Copy-on-write, `ImmutableDictionary`, or `FrozenDictionary` (rebuilt on change) are the usual tools.
2. `ReaderWriterLockSlim` is *not* an automatic fix: for very short critical sections its own overhead can be worse than a plain `lock`. Measure it (extra credit).
3. The write path pays: know the write rate and the object size before choosing.
4. Publication needs `volatile` (or `Volatile.Write`/`Interlocked.Exchange`) so readers see a fully-built object.

## Go further
Use `FrozenDictionary` rebuilt on write, and compare read speed. What if writes were 10% of operations instead of 0.005%?

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- Microsoft Learn: FrozenDictionary and ImmutableDictionary API docs *(title only)*
- Duffy, Concurrent Programming on Windows *(title only)*
