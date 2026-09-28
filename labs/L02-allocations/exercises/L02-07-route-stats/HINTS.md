# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type: what is allocated once per event? Then dotTrace: what does the loop call per event?
</details>

<details><summary>Hint 2: where?</summary>

The key is built per event with string interpolation, then used for `GetValueOrDefault` and the indexer: two hash lookups, both hashing the whole string, and the *key strings for repeated combinations are all thrown away* except the first.
</details>

<details><summary>Hint 3: why?</summary>

A composite string key means allocating and hashing a fresh string to identify something that is really three numbers. What type carries several values, has value equality and a good hash code, and lives inline without a heap object? (Then check: does the type you pick implement `IEquatable<T>`?)
</details>
