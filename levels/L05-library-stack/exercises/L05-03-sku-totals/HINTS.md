# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The profiler shows time in the SQLite native read path, not your code. Use the database's own tool: ask SQLite for the query plan (`EXPLAIN QUERY PLAN SELECT …`), or enable EF command logging to see the SQL.
</details>

<details><summary>Hint 2: where?</summary>

The plan says `SCAN Lines` (or `SCAN TABLE`) for a query that filters on one column. What would a *search* look like instead?
</details>

<details><summary>Hint 3: why?</summary>

Without an index the database reads all 200,000 rows for every query: 100 queries × 200k rows. An index on the filtered column lets it jump straight to matching rows. In EF you declare it in the model (`HasIndex`), and it gets created with the schema.
</details>
