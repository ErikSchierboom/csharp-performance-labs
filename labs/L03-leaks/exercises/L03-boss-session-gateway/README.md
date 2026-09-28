# L03-boss - Session gateway

*Everything Must Go (Nothing Does)*

## Symptom
A gateway handles 1,500 sessions per batch. After a batch **tens of megabytes stay reachable after a full GC**, though nothing is supposed to outlive its session. The growth has more than one shape: some of it is whole objects, some of it is buffers you don't remember holding. (Test scaffolding clears the shared state between runs; that is not the fix.)

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 4 ref-ms |
| Median allocated | 24 MB |
| Kept after a full GC | ≤ 1 MB |

## Final boss fight
This is the **final boss** of its lab: a disguised combination of that lab's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
