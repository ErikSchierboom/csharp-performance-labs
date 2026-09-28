# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Use *both* modes and compare. Sampling shows where the CPU *is*; tracing adds exact call counts. Neither can show a method the JIT has inlined: there's no call left to see, so its time is added to the caller's own time.
</details>

<details><summary>Hint 2: where?</summary>

In the sampling profile, look **inside** the dictionary lookup: what is the hot child, and is the *count* of `Equals` calls per lookup around one, or much higher?
</details>

<details><summary>Hint 3: why?</summary>

`Dictionary` relies on `GetHashCode` spreading keys across buckets. If many different keys share a hash, lookups degrade to walking a long chain of `Equals` calls. Check the key's `GetHashCode`. (Confirm the mechanism by counting `Equals` calls.)
</details>
