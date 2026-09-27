# L12-05 - Flaky downstream

*When It Rains*

## Symptom
A downstream service copes with up to 20 concurrent calls and fails fast above that - but "fails fast" still ties up one of its 25 workers for the call's full duration (a real overloaded backend doesn't reject for free: a connection, a thread, a timeout all cost something). Under 80 users **downstream calls balloon to several times the request count**, and many requests still fail (`giveUps`). Every one of those extra calls competes with genuine traffic for the same 25 workers, so **the retry storm doesn't just fail more - it is slower overall**, not merely noisier.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1000 ref-ms |
| Median allocated | 2 MB |
| Median p99 latency | 200 ref-ms |
| downstreamCalls | 400 |
| giveUps | 0 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
