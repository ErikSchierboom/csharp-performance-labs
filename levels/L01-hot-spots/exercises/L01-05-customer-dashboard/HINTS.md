# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling shows *where time goes* but not *how many times* something ran. Try **tracing** mode, which records
exact call counts. Compare the number of calls to the scoring method with the number of active customers.
</details>

<details><summary>Hint 2: where?</summary>

The scoring method is called a multiple of the number of active customers. Find every place in
`DashboardBuilder.Build` that consumes `scored`, and count them.
</details>

<details><summary>Hint 3: why?</summary>

`Where(...).Select(...)` doesn't compute anything when you write it: it describes a query. What happens
each time you ask that query a new question (`Any`, `Count`, `Sum`, `OrderBy`)?
Also: Rider/ReSharper flag this pattern statically ("possible multiple enumeration").
</details>
