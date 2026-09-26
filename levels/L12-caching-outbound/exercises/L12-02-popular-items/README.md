# L12-02 - Popular items

*Opening Night*

## Symptom
Five popular items are cached in `IMemoryCache` with a 50 ms loader. When the cache is cold (after a deploy, an eviction, or a restart), 40 concurrent requests arrive together and **every one of them runs the loader**: the harness's `loads` counts dozens of loads for 5 distinct keys, and the backend sees a spike exactly when the cache is empty.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 221 ref-ms |
| Median allocated | 2 MB |
| Median p99 latency | 204 ref-ms |
| loads | ≤ 9 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
