# L10-boss - Async quotes

*Quote, Unquote, Timeout*

## Symptom
A quote endpoint under 100 concurrent users has a **p99 in the hundreds of milliseconds** (a lone request takes ~30 ms), the CPU is idle, and a downstream service sees **hundreds of calls in flight at once**.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2164 ref-ms |
| Median allocated | 9 MB |
| Median p99 latency | 621 ref-ms |
| peakInflight | ≤ 61 |

## Final boss fight
The **final boss** of its lab: a disguised combination of that lab's defects with **no per-defect hints**. Profile, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
