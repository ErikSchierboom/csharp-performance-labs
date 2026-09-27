# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

EF command logging shows one query with two `LEFT JOIN`s; the harness's `rowsPerRequest` metric is the tell (it counts rows as `DbDataReader.Read()` returns them, before EF folds duplicates back into distinct entities). dotMemory: many duplicated materialised values.
</details>

<details><summary>Hint 2: where?</summary>

How many rows come back for one customer: orders + addresses, or orders × addresses?
</details>

<details><summary>Hint 3: why?</summary>

A single-query `Include` of two sibling collections joins them: rows = orders × addresses (a **cartesian explosion**), and each parent's columns repeat on every row. `AsSplitQuery()` issues one query per collection (2–3 small queries), or you can project only what you need.
</details>
