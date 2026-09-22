# L5-03 · SKU totals (missing index)

## Symptom
Totalling sales for 100 SKUs from a table of 200,000 rows takes **~0.5 s**. Each query is tiny and the C# is one line. Nothing allocates much and nothing looks wrong in the application code. The time is all spent *inside the database*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 64 ref-ms |
| Median allocated | 3 MB |

## Extra credit
Run `EXPLAIN QUERY PLAN` yourself against the seeded data (via a raw command) before and after you add the index.
