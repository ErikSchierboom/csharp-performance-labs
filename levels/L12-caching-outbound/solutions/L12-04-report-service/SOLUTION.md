# L12-04 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Handler invocations:** 1,200 (slow) vs ~20 (fix).

## Root cause
Identical, cacheable responses recomputed on every request.

## Fix
`AddOutputCache` + `UseOutputCache` + `.CacheOutput()` on the endpoint (with a sensible policy: expiration, vary-by, tags for invalidation).

## Take-aways
1. **The fastest request is one you don't run.** Cache at the highest layer that's correct.
2. Decide the invalidation story (expiry, tags, events) before you cache.
3. Vary on what changes the response (query, headers, user); an under-specified cache key serves wrong data.
4. Output caching helps only for responses that are safe to share; don't cache per-user or sensitive data without vary/authorisation.

## Extra credit
Add `[Authorize]`-like per-user variation (`VaryByHeader`) and see the hit rate drop.

## Go further
Add a policy with a 1-second expiration and a tag; evict by tag on a `POST`. What does p99 do at the expiry boundary?
