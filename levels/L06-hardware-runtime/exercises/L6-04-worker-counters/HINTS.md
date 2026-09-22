# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Timeline shows eight busy threads and no blocking. The evidence is at the hardware level: `perf stat` shows heavy cache-coherence traffic; `perf c2c` reports contended cache lines.
</details>

<details><summary>Hint 2: where?</summary>

Which memory does each thread write, and how far apart are those addresses? A `long` is 8 bytes.
</details>

<details><summary>Hint 3: why?</summary>

CPUs move memory in 64-byte cache lines, and only one core may hold a line for writing at a time. Eight adjacent `long`s share one line, so every increment steals the line from another core: **false sharing**. Separate the counters by at least a cache line.
</details>
