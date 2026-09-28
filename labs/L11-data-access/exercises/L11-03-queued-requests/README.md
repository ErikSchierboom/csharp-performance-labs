# L11-03 - Idle database, queued requests

*Waiting in Line*

## Symptom
The database is nowhere near busy (each query is 1 ms) but under 64 users **requests queue for a connection**, throughput plateaus around 250 requests/second and p99 is huge. In production the same shape shows up as `Timeout expired... all pooled connections were in use`. Threads and CPU are idle.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1330 ref-ms |
| Median allocated | 6 MB |
| Median p99 latency | 161 ref-ms |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
