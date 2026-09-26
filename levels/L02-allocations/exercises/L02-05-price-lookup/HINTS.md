# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations, grouped by type. Which types are allocated, and where do they come from?
</details>

<details><summary>Hint 2: where?</summary>

`Task<decimal>` and, on the rare slow path, async state machine objects. `GetPriceAsync` is declared `async`; look at what it does on the cache-hit path.
</details>

<details><summary>Hint 3: why?</summary>

An `async Task<T>` method has to return a `Task<T>`, and unless `T` is one of a few cached special cases (small ints, `true`/`false`) that is a **new heap object even when the method completes synchronously.** Which return type lets a synchronous result travel without a heap object?
</details>
