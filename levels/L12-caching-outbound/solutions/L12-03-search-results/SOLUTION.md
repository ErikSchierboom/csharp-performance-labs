# L12-03 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Retained:** thousands of strings rooted from the cache's entry table.

## Root cause
A cache with no size limit or expiry keyed by high-cardinality input: a memory leak by design (L03-02 in a real host).

## Fix
`SizeLimit` on the cache, `SetSize` on each entry, and an expiration. Monitor hit rate and eviction count.

## Take-aways
1. **Every cache needs a bound and an expiry**; choose them from memory budget and access pattern.
2. High-cardinality keys (search text, user ids, URLs) are the danger.
3. `SizeLimit` without `SetSize` throws: the framework forces you to think about entry cost.
4. Track `hit rate`: a bound that's too small turns the cache into pure overhead.

## Extra credit
Set `SizeLimit = 50` and re-run. What's the hit rate, and does it still help?

## Go further
Replace with `HybridCache` and configure `MaximumPayloadBytes`. Where does the bound live now?
