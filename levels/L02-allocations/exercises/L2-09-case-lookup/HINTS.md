# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by call site: `string` from `ToLowerInvariant`.
</details>

<details><summary>Hint 2: where?</summary>

Where is a new string created for every lookup, and is that string needed after the lookup?
</details>

<details><summary>Hint 3: why?</summary>

Normalising by copying is one way to get case-insensitive matching; the other is to teach the *comparer* to ignore case. `StringComparer.OrdinalIgnoreCase` (and the dictionary constructor that takes it) does both hashing and equality without allocating.
</details>
