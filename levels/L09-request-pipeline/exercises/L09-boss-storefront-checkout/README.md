# L09-boss - Storefront checkout

*Long Queue at Register One*

## Symptom
A checkout endpoint accepts a ~60 KB basket and answers with a receipt. **Per request it allocates hundreds of KB**, triggers gen2 collections, and takes far longer than the work justifies. A middleware, a DI call, the body read, and the response writing each look reasonable in isolation.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 934 ref-ms |
| Median allocated | 752 MB |
| Median gen2 collections | ≤ 2 |
| Median p99 latency | 44 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
An ASP.NET Core server runs on loopback **inside the harness process** (`WebRig`). Allocation and CPU include the small constant client cost.

## Final boss fight
This is the **final boss** of its level: a disguised combination of that level's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
