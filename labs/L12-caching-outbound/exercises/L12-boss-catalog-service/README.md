# L12-boss - Catalog service

*Cache Me Outside*

## Symptom
A product endpoint is fronted by a cache, yet the backend sees **a new TCP connection for almost every load**, repeated bursts of duplicate loads for the same hot product every time its cache entry expires, and after a run **~10 MB stays reachable after a full GC**: it grows with every one-off product id ever requested. Memory and connections both climb with traffic *variety*, and the repeated stampedes make it worse with every cache expiry, not just once at startup.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1800 ref-ms |
| Median allocated | 100 MB |
| Median p99 latency | 128 ref-ms |
| connections | ≤ 49 |
| Kept after a full GC | ≤ 11 MB |

> The exercise runs two separate ASP.NET Core servers on loopback **inside the harness process** (`WebRig`): the product service under test, and a catalogue backend it depends on. Allocation and CPU include the small constant client cost.

## Final boss fight
The **final boss** of its lab: a disguised combination of that lab's defects with **no per-defect hints**. Profile, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
