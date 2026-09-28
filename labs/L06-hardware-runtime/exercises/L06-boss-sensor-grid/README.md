# L06-boss - Sensor grid

*Know Your Silicon*

## Symptom
A sensor-processing step computes column totals of a 2048×2048 grid, a weighted sum over one million sparse readings, and a count of a specific code among eight million. Together they take **~30 ms**; the same three loops "should" take a fraction of that. There are no allocations in the hot path and no obviously slow calls.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 7 ref-ms |
| Median allocated | 0 MB |

## Final boss fight
This is the **final boss** of its lab: a **disguised combination** of defects from this lab, in a different domain. There are no per-defect hints. Profile it, list what you find, fix one thing at a time, and afterwards write down **which earlier exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
