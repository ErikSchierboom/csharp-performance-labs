# L01-boss - Order ledger

*Death by a Thousand Cuts*

## Symptom
A ledger job parses 10,000 order lines, totals them, and writes a report.  Several separate costs are stacked on top of each other.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 9 MB |

## Final boss fight
This is the **final boss** of its lab: a disguised combination of that lab's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
