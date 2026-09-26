# L12-02 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Metric:** `loads` ≈ dozens (slow) vs exactly 5 (fix).

## Root cause
Non-atomic get-or-create in a hot cache: every concurrent miss recomputes the same value (thundering herd).

## Fix
Single-flight per key with `Lazy<Task<T>>` in a `ConcurrentDictionary` (as here), or `HybridCache.GetOrCreateAsync`. Add expiry and decide what happens on failure (don't cache a faulted task forever).

## Take-aways
1. **A cache miss under load is a synchronised event.** Warm caches before taking traffic, and protect the loader.
2. This is the seed of many incidents: cold cache + burst → backend overload → timeouts → retries → more load.
3. `HybridCache` gives stampede protection and L1+L2 caching; prefer it over hand-rolled code.

## Extra credit
Add a 100 ms expiry and re-run. What does the *periodic* re-load do to p99? (This is the seed of L14-04.)

## Go further
Try `HybridCache` (needs the `Microsoft.Extensions.Caching.Hybrid` package) and compare `loads`.
