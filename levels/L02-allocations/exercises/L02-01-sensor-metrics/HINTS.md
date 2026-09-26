# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Use dotMemory's **allocation** view (not just dotTrace). Look at the allocations grouped **by type**. What is the top type, and does it match anything in the *source data*?
</details>

<details><summary>Hint 2: where?</summary>

The top type is a primitive. Find the code that stores or fetches values through `object`. There are three separate places, not one: two collections, and one operation on one of them.
</details>

<details><summary>Hint 3: why?</summary>

Putting an `int` into an `object` slot *boxes* it: a new heap object, 24 bytes for a 4-byte value. Non-generic collections (`ArrayList`, `Hashtable`) store `object`. Sorting them also goes through `IComparable`. Which generic collections keep the values as `int`?
</details>
