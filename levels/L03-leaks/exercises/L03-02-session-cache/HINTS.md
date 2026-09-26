# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory snapshot **Compare** across a run. Which type accounts for the growth, and what is the *dominator* that holds them?
</details>

<details><summary>Hint 2: where?</summary>

The dominator is one static collection. Read its declaration and every place that adds to it. Is there any place that removes entries?
</details>

<details><summary>Hint 3: why?</summary>

A cache with no eviction policy is a leak with a friendly name. What is the *working set* the code actually needs (how far back do lookups reach)? Choose a bound from that, then choose an eviction policy: size (LRU), age (TTL), or both.
</details>
