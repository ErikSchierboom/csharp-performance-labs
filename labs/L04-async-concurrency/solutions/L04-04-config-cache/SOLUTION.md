# L04-04 - Solution

## What the profile shows

- **Metric:** `factoryCalls` ≈ 20 x (up to 8) in the slow version; exactly 20 in the fix.
- **CPU time:** ~8x the fix, since each round's 8 threads all burn 5 ms.

## Root cause
`ConcurrentDictionary.GetOrAdd(key, Func)` does not lock while running the factory. Threads that miss simultaneously each run it; only the first result is stored. For expensive or side-effecting factories that's redundant work (and, with side effects, potentially wrong behaviour).

## Fix
Store `Lazy<T>` values: `GetOrAdd(key, k => new Lazy<T>(() => Load(k))).Value`. Several threads may create a `Lazy` (cheap), but the default thread-safety mode guarantees the loader runs once. (Async version: cache a `Lazy<Task<T>>` or use `HybridCache`.)

## Take-aways
1. **`GetOrAdd` is not 'compute once'.** Only the *stored value* is unique.
2. Wasted work often shows in CPU and load on dependencies, not in latency; measure both.
3. `Lazy<T>` modes: `ExecutionAndPublication` (default, runs once), `PublicationOnly` (may run many), `None` (not thread-safe).
4. This is the seed of a **cache stampede** (Lab 12): many misses recompute the same expensive value at once.

## Extra credit
Print `factoryCalls` for 2, 8 and 32 threads. What does the number do, and why isn't it always 20 × threads?

## Go further
Make the loader `async` and cache `Lazy<Task<int>>`. What happens if the task faults: is the failure cached forever?
