# L2-05 · Solution

## What the profile shows
> **About the profile section.** I have not captured Rider/dotTrace/dotMemory output for this exercise. What is described is what the code and the harness numbers imply. The harness figures (time, MB, GC counts) were measured. Where your profiler shows something different, trust the profiler and note the difference in `templates/LAB-LOG.md`.

- **dotMemory allocations:** ~2,000,000 `System.Threading.Tasks.Task<decimal>` instances, one per call. On the rare miss, the state machine box appears too.
- **dotTrace:** nothing "slow" stands out: `AsyncTaskMethodBuilder` frames only. It is death by small allocation, not by CPU hot spot.

## Root cause
`async Task<decimal>` returns a `Task<decimal>` object on **every** call, even when `TryGetValue` succeeds and the method never awaits. Two million calls is two million tasks. There's no cached `Task<decimal>` for arbitrary decimal values, so each result gets its own.

## Fix
Return `ValueTask<decimal>`. On the hot path return `new ValueTask<decimal>(cached)`, a struct with the value inside and no heap allocation. Only the rare miss path needs a real `Task`, so move it to a separate `async Task<decimal>` helper and wrap it. (A non-`async` method returning `ValueTask` directly avoids the state machine on the fast path too.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before (`async Task<decimal>`) | ≈ 49 ms | 152.7 MB |
| after (`ValueTask<decimal>`) | ≈ 30 ms | 0.09 MB |

Absolute times depend on your machine and runtime version; the *ratios* and the allocation numbers should look similar.

## Take-aways
1. `async` does not make anything faster; it makes *waiting* cheap. If the method usually doesn't wait, you are paying for machinery you don't use.
2. **`ValueTask` has rules:** await it once, don't block on it, don't keep it around. When in doubt convert with `.AsTask()`. The default choice for public APIs is still `Task`; use `ValueTask` where a profile shows the allocation matters.
3. Alternative fix: cache the `Task<decimal>` per key (`Task.FromResult` once). It allocates nothing per call, and any number of callers can await it.
4. On the true async path (`Task.Yield()` here) `ValueTask` costs about the same as `Task`: its win is on the synchronous path.

## Go further
What if the cache were 50% misses? Measure both `Task` and `ValueTask` at hit rates of 100%, 90% and 50%. Where does the advantage disappear?

## Further reading
- [Toub, Understanding the Whys, Whats, and Whens of ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)
- [Toub, How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)
- [Toub, Async ValueTask Pooling in .NET 5](https://devblogs.microsoft.com/dotnet/async-valuetask-pooling-in-net-5/)
