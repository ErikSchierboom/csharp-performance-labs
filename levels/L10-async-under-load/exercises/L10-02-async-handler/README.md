# L10-02 - Async handler

*Busy Doing Nothing*

## Symptom
The handler is declared `async` and awaits something, so it *looks* non-blocking. But under 200 concurrent users its p99 is far above the 15 ms of "work", and the CPU is idle. Throughput is capped at a small multiple of the pool's thread count.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 309 ref-ms |
| Median allocated | 7 MB |
| Median p99 latency | 83 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.
