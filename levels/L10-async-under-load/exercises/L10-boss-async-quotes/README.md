# L10 · Async quotes (final boss of Level 10)

## Symptom
A quote endpoint under 100 concurrent users has a **p99 in the hundreds of milliseconds** (a lone request takes ~30 ms), the CPU is idle, and a downstream service sees **hundreds of calls in flight at once**. Threads are busy waiting, not working.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2164 ref-ms |
| Median allocated | 9 MB |
| Median p99 latency | 621 ref-ms |
| peakInflight | ≤ 61 |

## Note (harness)
An ASP.NET Core server runs on loopback **inside the harness process** (`WebRig`). Allocation and CPU include the small constant client cost.

## Final boss fight
The **final boss** of its level: a disguised combination of that level's defects with **no per-defect hints**. Profile, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.

## Extra credit
Which fix moves p99 the most? `peakInflight` the most?
