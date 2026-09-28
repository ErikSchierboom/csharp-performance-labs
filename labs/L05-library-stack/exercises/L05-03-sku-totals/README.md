# L05-03 - SKU totals

*The Long Tally*

## Symptom
Totalling sales for 100 SKUs from a table of 200,000 rows takes **~0.5 s**. Each query is tiny and the C# is one line. Nothing allocates much and nothing looks wrong in the application code. The time is all spent *inside the database*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 25 ref-ms |
| Median allocated | 2 MB |
