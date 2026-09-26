# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotTrace **timeline** shows threads mostly *waiting on a monitor*; `dotnet-counters` `monitor-lock-contention-count` climbs quickly. Sampling shows the time in `Audit`, but only one thread at a time.
</details>

<details><summary>Hint 2: where?</summary>

All eight threads queue at one `lock`. Look at *what is executed while holding it*: which of those statements actually needs the lock?
</details>

<details><summary>Hint 3: why?</summary>

A lock serialises whatever is inside it. Work that depends only on its argument doesn't need protection; only the shared write does. Can the shared write be made safe without a lock (a single atomic operation)?
</details>
