# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling, sorted by self time. Then tracing mode to see how many times `Sort` is called and with how many elements.
</details>

<details><summary>Hint 2: where?</summary>

The top frame is inside `List<T>.Sort`. Walk up to your loop: how many times is it called, and how large is the list each time?
</details>

<details><summary>Hint 3: why?</summary>

Sorting n elements costs O(n log n); doing it after *every* insert is O(n² log n) overall. What does the loop actually *read*, and what's the cheapest structure that answers exactly that question?
</details>
