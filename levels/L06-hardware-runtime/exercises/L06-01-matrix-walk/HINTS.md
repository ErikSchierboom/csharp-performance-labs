# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The sampling profiler will tell you *where* (that one line) but not *why*. Time to hypothesise about the **hardware**. On Linux, `perf stat -e cache-misses,cache-references` around the run gives evidence; BenchmarkDotNet can't read hardware counters on Linux.
</details>

<details><summary>Hint 2: where?</summary>

Look at how the two loops index the array: which index changes fastest, and how far apart in memory are consecutive accesses?
</details>

<details><summary>Hint 3: why?</summary>

Memory is loaded in 64-byte cache lines. Walking down a column jumps 4096 × 4 bytes = 16 KB per step, so every access touches a different cache line (and evicts before you return to it). Walking along a row uses all 16 ints in each line, and the hardware prefetcher can stream ahead.
</details>
