# L3-boss · Session gateway (final boss of Level 3)

## Symptom
A gateway handles 1,500 sessions per batch. After a batch **tens of megabytes stay reachable after a full GC**, though nothing is supposed to outlive its session. The growth has more than one shape: some of it is whole objects, some of it is buffers you don't remember holding. (Test scaffolding clears the shared state between runs; that is not the fix.)

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 10 ref-ms |
| Median allocated | 59 MB |
| Kept after a full GC | ≤ 3 MB |

## Final boss fight
This is the **final boss** of its level: a disguised combination of that level's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.

## Extra credit
Break your own fix: what happens if the lookback grows to 500 entries?
