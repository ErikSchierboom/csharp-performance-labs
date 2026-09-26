# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The profiler shows the loop. `perf stat -e branches,branch-misses` shows the story: a large fraction of branches mispredicted.
</details>

<details><summary>Hint 2: where?</summary>

There is exactly one data-dependent branch in the loop. How predictable is it when the data is random? When the data is sorted?
</details>

<details><summary>Hint 3: why?</summary>

A modern CPU guesses branch outcomes to keep its pipeline full; a wrong guess flushes ~15–20 cycles of work. Random 50/50 data is unpredictable (about half of the branches mispredict); sorted data is almost perfectly predictable. The analysis doesn't depend on the order of the readings, so you may reorder them.
</details>
