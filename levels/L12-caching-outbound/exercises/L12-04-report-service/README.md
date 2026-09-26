# L12-04 - Report service

*Hot Off the Press*

## Symptom
A report endpoint has only 20 distinct outputs and they change rarely, yet every request recomputes the 8 ms report. At 32 users the endpoint's throughput is bounded by the report's cost and the backend is doing the same work over and over.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 38 ref-ms |
| Median allocated | 14 MB |
| Median p99 latency | 6 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
