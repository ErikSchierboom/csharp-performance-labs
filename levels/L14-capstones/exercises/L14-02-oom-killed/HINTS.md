# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The harness's kept-after-GC and gen2 lines; `dotnet-counters`: `gen-2 collections` and LOH size; dotMemory: what dominates?
</details>

<details><summary>Hint 2: where?</summary>

Two things allocate 100 KB per request: what is done with them afterwards?
</details>

<details><summary>Hint 3: why?</summary>

Each request allocates a 100 KB buffer (LOH: L02-03, gen2 storms) **and** stores it in a cache with no bound (L03-02, L12-03), so live memory grows without limit until the container's memory limit kills the process. Fix both: pool scratch buffers, cache a small derived value, bound the cache.
</details>
