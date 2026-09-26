# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Only time is over budget, memory isn't. That points to a **sampling** CPU profile, not a memory profiler.
</details>

<details><summary>Hint 2: where?</summary>

Look at the top self-time frames and walk *up* to the first frame that is your code. Then ask: how many
times is that frame's callee invoked per row? Is that number constant, or does it change as the import
progresses?
</details>

<details><summary>Hint 3: why?</summary>

For every row you scan a collection that keeps growing. Total work ≈ rows × (average size of the collection).
What data structure answers "have I seen this already?" in roughly constant time, while still honouring
case-insensitive comparison?
</details>
