# L12-05 - Flaky downstream

*When It Rains*

## Symptom
A downstream service copes with up to 20 concurrent calls and fails fast above that. Under 80 users **downstream calls balloon to several times the request count**, and many requests still fail (`giveUps`). Latency looks fine at this scale, because failures are fast and served as a fallback. That is exactly why it is dangerous: **the damage is in the counts** (extra calls, give-ups). In production, the extra load is what keeps the downstream down. A brief overload has become a sustained one.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 3246 ref-ms |
| Median allocated | 4 MB |
| Median p99 latency | 724 ref-ms |
| downstreamCalls | ≤ 601 |
| giveUps | ≤ 1 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
