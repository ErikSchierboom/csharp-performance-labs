# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Wall time hides this, because the duplicates run in parallel. Look at *CPU time* (the harness reports it) and at the **call count** of `LoadSection` (tracing mode, or the `factoryCalls` metric).
</details>

<details><summary>Hint 2: where?</summary>

`ConcurrentDictionary.GetOrAdd(key, factory)` is thread-safe for the *dictionary*, but read what the documentation says about the *factory*.
</details>

<details><summary>Hint 3: why?</summary>

`GetOrAdd` can call the factory on several threads at once and then keep only one result. The others' work is thrown away. What can you store in the dictionary that defers the expensive work, and guarantees it runs once?
</details>
