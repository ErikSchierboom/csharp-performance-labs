# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Timeline first (idle CPU, slow requests = waiting), then `dotnet-counters` for thread-pool and lock-contention counters. Only then sampling.
</details>

<details><summary>Hint 2: where?</summary>

Follow one request: what is it waiting *for*, and who is that holding? When you fix the first thing you find, re-measure: the next problem will look different (contention, duplicated work, sequential waiting).
</details>

<details><summary>Hint 3: why?</summary>

Layers of the earlier levels are stacked here: blocking on async, a lock held over a slow operation, misses that all recompute the same thing, and sequential awaits that could be concurrent. Fix in the order the evidence exposes them.
</details>
