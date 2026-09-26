# L12-boss - Catalog service

*Cache Me Outside*

## Symptom
A product endpoint is fronted by a cache, yet the backend sees **a new TCP connection for almost every load**, cold-start bursts of duplicate loads for the same hot product, and after a run **~10 MB stays reachable after a full GC**: it grows with every one-off product id ever requested. Memory and connections both climb with traffic *variety*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1309 ref-ms |
| Median allocated | 153 MB |
| Median p99 latency | 145 ref-ms |
| connections | ≤ 49 |
| Kept after a full GC | ≤ 11 MB |

## Note (harness)
An ASP.NET Core server runs on loopback **inside the harness process** (`WebRig`). Allocation and CPU include the small constant client cost.

## Final boss fight
The **final boss** of its level: a disguised combination of that level's defects with **no per-defect hints**. Profile, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
