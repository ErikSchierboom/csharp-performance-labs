# L12 · Unbounded MemoryCache

## Symptom
An endpoint caches search results by query. With 3,000 distinct queries in a run, about **24 MB stays reachable after a full GC** and it keeps growing with traffic variety: on a real site with millions of distinct queries it grows until the container is killed. `MemoryCache` is a 'proper' cache with no bound configured.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 106 ref-ms |
| Median allocated | 168 MB |
| Median p99 latency | 6 ref-ms |
| Kept after a full GC | ≤ 9 MB |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.

## Extra credit
Set `SizeLimit = 50` and re-run. What's the hit rate, and does it still help?
