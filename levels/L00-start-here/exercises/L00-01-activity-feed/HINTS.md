# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling profile, Release. Sort by **own time**. Allocation is tiny, so dotMemory has nothing to say: this is a CPU problem.
</details>

<details><summary>Hint 2: where?</summary>

The top self-time frame is not in your code. Walk *up* the call tree to the first frame that is: which line of `Feed.Build` is it, and what BCL method does it call?
</details>

<details><summary>Hint 3: why?</summary>

`List<T>` is an array underneath. Inserting at index 0 has to move every existing element one slot to the right. How many elements are moved by the 1st insert? By the 60,000th? Add those up.
</details>
