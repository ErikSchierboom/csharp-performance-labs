# L7-01 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** threads blocked on one monitor and on `Task.Wait`; thread count creeping up; near-zero CPU; the database sees one call at a time.
- After removing the lock: many identical database calls for the same SKU (stampede). After that: 20 sequential round trips per request.

## Root cause
Four stacked defects. (1) The cache loads **while holding a lock**, so every miss in the process serialises. (2) It loads with **sync-over-async** (`.Result`) on a pool thread, so blocked threads starve the pool. (3) Once the lock is gone, concurrent misses for the *same* SKU each hit the database (**stampede**). (4) A request awaits its 20 prices **one after another** instead of concurrently, and builds the receipt by string concatenation.

## Fix
`ConcurrentDictionary<int, Lazy<Task<int>>>` so each SKU is loaded once and shared, awaited (no blocking, no lock); look up a request's 20 prices with `Task.WhenAll`; build the receipt with a `StringBuilder`. Each step is a fix from Level 4 (and L12's stampede lesson).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 406 ms | 0.37 MB | ≈ 406 ms |
| after | ≈ 3.5 ms | 0.35 MB | ≈ 4 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Real incidents stack.** The first fix rarely finishes the job; it moves the bottleneck. Re-measure after every change.
2. Each layer had a distinct signature (blocked-on-monitor, starvation, duplicate calls, sequential waits); recognising them quickly is the payoff of the earlier levels.
3. Caching hides load until it doesn't: an empty cache under a burst is exactly when misses and stampedes hurt.
4. Write the post-mortem: which defect masked which? That ordering is the lesson.

## Go further
Add a bounded degree of parallelism per request (e.g. `SemaphoreSlim(8)`) and a global limit for the database, and explain when each is needed.

## Further reading
- [Kokosa et al., Pro .NET Memory Management](https://prodotnetmemory.com/)
- [Gregg, Systems Performance (the USE method)](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)
- templates/POSTMORTEM.md (this repo): write yours first
