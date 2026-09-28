# L11-04 - Paged catalogue

*Turn the Page*

## Symptom
An endpoint serves a paged product catalogue. After 400 requests, about **30 MB is still reachable after a full GC**, and it grows with every distinct row ever read. Under concurrency, requests also queue up behind each other, although the database is idle. Nothing in the code looks like a cache.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 60 ref-ms |
| Median allocated | 70 MB |
| Median p99 latency | 5 ref-ms |
| Kept after a full GC | 0 MB |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
