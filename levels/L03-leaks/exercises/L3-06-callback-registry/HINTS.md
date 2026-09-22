# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory snapshot compare: what grew? The `byte[]` (20,000) count matches the number of callbacks.
</details>

<details><summary>Hint 2: where?</summary>

Follow the retention path from a `byte[]`: registry → delegate → **closure object** → array. What is in the closure?
</details>

<details><summary>Hint 3: why?</summary>

A lambda captures the *variables it uses* in a compiler-generated closure object that lives as long as the delegate. `report` was captured because the lambda reads it. Capture only the small value you need (copy it to a local first) and the big array can be collected.
</details>
