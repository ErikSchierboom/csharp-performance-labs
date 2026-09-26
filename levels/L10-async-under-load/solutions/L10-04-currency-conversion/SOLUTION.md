# L10-04 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Counters:** healthy pool, low CPU. **Outbound calls:** ≈ one per request; **queue:** requests waiting on `SemaphoreSlim`.

## Root cause
A global `SemaphoreSlim(1)` serialises a slow, cacheable remote call for every request, so throughput is capped at one call time and every request pays the queue.

## Fix
Cache the fetch per currency as a shared `Lazy<Task<decimal>>` (one call per key, awaited by all). In production add expiry, refresh-ahead and failure handling (`HybridCache` does much of this).

## Take-aways
1. **An async lock removes thread blocking but not serialisation.** A gate on the hot path caps throughput.
2. If the data is shared and slow-changing, **cache it** and dedupe concurrent fetches (single-flight).
3. Decide what happens on failure and on expiry: a cached faulted task must not be cached forever.
4. Lock *refresh*, not *reads*.

## Extra credit
What if the rate can differ per *user* (many keys)? What would you use to bound the cache size?

## Go further
Add a 5-second expiry with refresh-ahead: serve the stale value while one request refreshes. What can go wrong if the refresh fails?
