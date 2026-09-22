# L4-02 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** 7 of 8 threads blocked on the monitor at any moment; contention count in the hundreds of thousands.
- **Sampling:** `Audit` dominates self time, and the total CPU is ≈ 1 core.

## Root cause
The lock's critical section includes the expensive `Audit` call, which only needs its argument. All threads take turns doing that work, so parallel workers behave like one thread plus lock overhead (and extra context switching when the lock is contended).

## Fix
Shrink the critical section to the smallest thing that must be atomic: compute `Audit` outside, then update the shared bucket with `Interlocked.Add` (no lock). Other options: per-thread buckets merged at the end (no sharing at all) or striped locks.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 134 ms | 0.00 MB |
| after | ≈ 23 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Hold locks for the shortest possible time**, and never across work that doesn't touch shared state (or across I/O or `await`).
2. Contention shows as *waiting*, not CPU: low CPU with many threads means look at the timeline and lock counters.
3. `Interlocked` and per-thread accumulation beat locks for counters; measure, because false sharing (Level 6) can bite the striped version.
4. Correctness first: the checksum verifies the atomic version still adds up.

## Go further
Implement per-thread buckets (`ThreadLocal<long[]>` or `Parallel.For` with local state) merged at the end. Compare it with `Interlocked` at 8 and at 32 workers.

## Further reading
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Cleary, Concurrency in C# Cookbook, 2nd ed.](https://stephencleary.com/book/)
- [Fowler, AspNetCoreDiagnosticScenarios: AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- Microsoft Learn: Overview of synchronization primitives *(title only)*
