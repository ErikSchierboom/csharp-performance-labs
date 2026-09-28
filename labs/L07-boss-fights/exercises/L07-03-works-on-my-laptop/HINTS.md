# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Compare the harness's first lines (GC mode, cores) and `committedMB` at different core counts (`taskset -c 0-1`, `0-3`, all), and read `bin/.../*.runtimeconfig.json` in the output folder. `dotnet-counters`: `gc-committed`, `gen-0-gc-count`, and `gc-heap-size`.
</details>

<details><summary>Hint 2: where?</summary>

There is no C# to change. Which *configuration* controls how many heaps the GC creates and how big each one's allocation budget is?
</details>

<details><summary>Hint 3: why?</summary>

Server GC creates one heap (and one GC thread) per core with a large per-heap gen0 budget: excellent throughput on a big machine, but a large memory footprint even for a small workload, especially with dynamic adaptation (DATAS) switched off. Options: fewer heaps (`GCHeapCount`), dynamic adaptation, or Workstation GC.
</details>
