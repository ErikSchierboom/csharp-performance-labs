# L12-03 - Search results

*Seek and You Shall Find*

## Symptom
An endpoint caches search results by query. With 3,000 distinct queries in a run, about **24 MB stays reachable after a full GC** and it keeps growing with traffic variety: on a real site with millions of distinct queries it grows until the container is killed. The code uses a proper cache library, not a hand-rolled dictionary.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 50 ref-ms |
| Median allocated | 70 MB |
| Median p99 latency | 1 ref-ms |
| Kept after a full GC | ≤ 5 MB |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
