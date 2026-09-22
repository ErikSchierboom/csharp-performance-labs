# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type: which types dominate, and how do they compare with the size of the *result*?
</details>

<details><summary>Hint 2: where?</summary>

Look at the first statement inside the loop: how many rows and columns does it fetch, and where does the filter run: in SQL, or in C#?
</details>

<details><summary>Hint 3: why?</summary>

Three costs stack up: fetching *all rows* (filter in memory), fetching *all columns* (the 1 KB description), and **change tracking** (EF keeps a snapshot of every entity so it can detect updates you never make). How would you ask for exactly the rows and columns needed, and turn tracking off?
</details>
