# L02-boss - Shipment manifest

*Careful Code, Careless Heap*

## Symptom
A nightly job turns 20,000 shipment lines into a manifest and a weight total. It takes **about 30 ms and allocates 150 MB**, and both numbers are worse than the task deserves. The code reads like ordinary, careful C#.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 1 MB |

## Final boss fight
This is the **final boss** of its lab: a **disguised combination** of defects from this lab, in a different domain. There are no per-defect hints. Profile it, list what you find, fix one thing at a time, and afterwards write down **which earlier exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
