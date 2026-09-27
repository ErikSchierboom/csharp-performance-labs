# L12-02 - Popular items

*Opening Night*

## Symptom
Five popular items are cached in `IMemoryCache` with a 50 ms loader. When the cache is cold (after a deploy, an eviction, or a restart), 40 concurrent requests arrive together and **every one of them runs the loader**: the harness's `loads` counts dozens of loads for 5 distinct keys. The backend behind the loader can only serve 8 calls at once (a connection pool, a rate limit); the rest queue. 40 redundant loads instead of 5 means extra queueing, so every caller - not just the redundant ones - waits longer.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 120 ref-ms |
| Median allocated | 1 MB |
| Median p99 latency | 120 ref-ms |
| loads | ≤ 9 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
