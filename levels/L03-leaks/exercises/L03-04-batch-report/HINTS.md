# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

First question: does memory *stay* reachable? Force a full GC and look at what remains (the harness's *kept* line, or a dotMemory snapshot after *Force GC*). Then look at the gen2 count.
</details>

<details><summary>Hint 2: where?</summary>

The *kept* number is ~0, so nothing is holding the temporary arrays: the process's memory figure was garbage that hadn't been collected yet. Which line makes every batch pay for a full, blocking collection?
</details>

<details><summary>Hint 3: why?</summary>

The GC collects when it needs memory, not when your code is done with an object. Garbage sitting in memory is normal, so a rising line on a memory dashboard is not evidence of a leak; retained size after a full GC is. `GC.Collect()` in production code almost always makes things slower.
</details>
