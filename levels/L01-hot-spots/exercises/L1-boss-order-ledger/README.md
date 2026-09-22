# L1-boss · Order ledger (final boss of Level 1)

## Symptom
A ledger job parses 5,000 order lines, totals them, and writes a report. It takes **about a second** for what is a few milliseconds of real work. The code is short, reads sensibly, and no single line looks outrageous. Several separate costs are stacked on top of each other.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 8 ref-ms |
| Median allocated | 9 MB |

## Final boss fight
This is the **final boss** of its level: a disguised combination of that level's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.

## Extra credit
Which single fix removes the most time? Which removes the most allocation?
