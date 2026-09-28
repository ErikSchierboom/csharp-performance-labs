# L13-02 - Request logging

*Dear Diary*

## Symptom
The service logs four lines per request through a small custom file logger. Latency is not what it should be.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 53 ref-ms |
| Median allocated | 25 MB |
| Median p99 latency | 7 ref-ms |

## Note (the ASP.NET Core labs (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Allocation and CPU include the small constant client cost.
