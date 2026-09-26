# L13-02 - Request logging

*Dear Diary*

## Symptom
The service logs four lines per request through a small custom file logger. At 32 users **p99 is dominated by logging**. The logging *volume* is modest; the way it's written is the cost. Removing the log calls makes the endpoint several times faster.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 53 ref-ms |
| Median allocated | 25 MB |
| Median p99 latency | 7 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Allocation and CPU include the small constant client cost.
