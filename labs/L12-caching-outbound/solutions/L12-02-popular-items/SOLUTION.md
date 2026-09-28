# L12-02 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

Measured on the author's machine (unscaled):

| | Slow | Fixed |
|---|---|---|
| Median time | ≈ 252 ms | ≈ 51 ms |
| Median p99 | ≈ 251 ms | ≈ 51 ms |
| `loads` | 40 | 5 |

- **Metric:** `loads` 40 (slow) vs exactly 5 (fix): dozens in a real system, exactly 40 here because every one of the 200 requests lands during the cold-cache window.

## Root cause
Non-atomic get-or-create in a hot cache: every concurrent miss recomputes the same value (thundering herd). The backend behind the loader can only serve 8 calls at once, so 40 redundant loads queue in waves of 8 - about 5 waves x 50 ms ≈ 250 ms - while 5 real loads fit in one wave and finish in ≈ 50 ms. The wasted work isn't just extra backend load: it directly inflates the latency every caller sees, including the ones whose key was never redundant.

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
