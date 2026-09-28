# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations, plus the harness columns: gen0, gen1 and gen2 are **all** equal to each other. Ordinary short-lived garbage would give lots of gen0 and few gen2. What kind of allocation makes every collection a gen2?
</details>

<details><summary>Hint 2: where?</summary>

The allocation is `new byte[PageBytes]` in `Render`. Look up the size threshold for the Large Object Heap and compare it with `PageBytes`.
</details>

<details><summary>Hint 3: why?</summary>

Objects of 85,000 bytes or more go on the LOH, which is collected only with gen2 collections. Allocating one per call means constant gen2 GCs. The array is needed only for the duration of the call. Which BCL type lends out arrays? Read its documentation about what a *rented* array contains, and how long it is.
</details>
