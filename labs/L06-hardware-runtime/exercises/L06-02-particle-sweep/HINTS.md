# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling shows the summation line hot again. Ask the hardware: `perf stat` cache-miss counts, or compare against a benchmark of the same loop over data that is *sequential*.
</details>

<details><summary>Hint 2: where?</summary>

An array of a *class* is an array of **references**. What does each iteration have to do before it can read `p.X`, and where might that object live?
</details>

<details><summary>Hint 3: why?</summary>

Each element is a pointer to a separate heap object; after allocation order is scrambled (as in any long-running program), consecutive elements point to unrelated cache lines: a dependent load per element that the prefetcher can't predict. A `struct` array stores the fields **inline**, contiguously.
</details>
