# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Count loader invocations vs distinct keys (`loads`), and backend load over time after a cache flush.
</details>

<details><summary>Hint 2: where?</summary>

`GetOrCreateAsync` checks the cache, and if missing runs the factory and stores the result. What happens when 8 requests check at the same moment?
</details>

<details><summary>Hint 3: why?</summary>

`IMemoryCache.GetOrCreate*` is **not atomic**: concurrent misses each run the factory (this is L4-04 at request scale). You need single-flight per key: one shared in-flight task per key (`ConcurrentDictionary<key, Lazy<Task>>`), a per-key lock, or `HybridCache`, which has stampede protection built in.
</details>
