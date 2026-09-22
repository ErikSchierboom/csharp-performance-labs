# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

`perf stat` for branch-misses; disassembly to see the loop.
</details>

<details><summary>Hint 2: where?</summary>

How many iterations does the inner loop run for a random 64-bit word, and is its exit predictable?
</details>

<details><summary>Hint 3: why?</summary>

Modern CPUs have a `POPCNT` instruction and .NET exposes it as `BitOperations.PopCount` (with a software fallback). Check `System.Numerics.BitOperations` before hand-writing bit tricks.
</details>
