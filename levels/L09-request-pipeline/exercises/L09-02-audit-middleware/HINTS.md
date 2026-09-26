# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations grouped by **call site**; dotTrace sampling. Look inside the middleware delegate, not the endpoint.
</details>

<details><summary>Hint 2: where?</summary>

List everything the middleware constructs or formats per request: what is *identical* on every request, and what happens to the log message when the level is off?
</details>

<details><summary>Hint 3: why?</summary>

A `Regex` built per request repeats parsing and setup work (Level 1: L01-03); an interpolated log message is formatted before the logger checks the level (Level 5: L05-06). Both are per-request costs that don't depend on the request.
</details>
