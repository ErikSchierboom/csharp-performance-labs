# L05-boss - Order report

*The Full Stack of Mistakes*

## Symptom
A nightly report totals each customer's orders and writes an audit line per customer. For 300 customers it takes **~50 ms and allocates ~8 MB**: far more than the data (1,800 order rows) warrants. From the outside you'd see hundreds of near-identical SQL statements, a lot of memory for a small result, and a thousand file operations.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2 ref-ms |
| Median allocated | 1 MB |

## Final boss fight
This is the **final boss** of its level: a disguised combination of that level's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
