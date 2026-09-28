# L02-05 - Solution

## What the profile shows

- **allocations:** ~2,000,000 `System.Threading.Tasks.Task<decimal>` instances, one per call. On the rare miss, the state machine box appears too.
- **tracing:** nothing "slow" stands out: `AsyncTaskMethodBuilder` frames only. It is death by small allocation, not by CPU hot spot.

## Root cause
`async Task<decimal>` returns a `Task<decimal>` object on **every** call, even when `TryGetValue` succeeds and the method never awaits. Two million calls is two million tasks. There's no cached `Task<decimal>` for arbitrary decimal values, so each result gets its own.

## Fix
Return `ValueTask<decimal>`. On the hot path return `new ValueTask<decimal>(cached)`, a struct with the value inside and no heap allocation. Only the rare miss path needs a real `Task`, so move it to a separate `async Task<decimal>` helper and wrap it. (A non-`async` method returning `ValueTask` directly avoids the state machine on the fast path too.)

## Take-aways
1. `async` does not make anything faster; it makes *waiting* cheap. If the method usually doesn't wait, you are paying for machinery you don't use.
2. **`ValueTask` has rules:** await it once, don't block on it, don't keep it around. When in doubt convert with `.AsTask()`. The default choice for public APIs is still `Task`; use `ValueTask` where a profile shows the allocation matters.
3. Alternative fix: cache the `Task<decimal>` per key (`Task.FromResult` once). It allocates nothing per call, and any number of callers can await it.
4. On the true async path (`Task.Yield()` here) `ValueTask` costs about the same as `Task`: its win is on the synchronous path.

## Go further
What if the cache were 50% misses? Measure both `Task` and `ValueTask` at hit rates of 100%, 90% and 50%. Where does the advantage disappear?
