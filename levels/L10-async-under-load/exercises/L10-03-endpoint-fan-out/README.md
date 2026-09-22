# L10 · Endpoint fan-out

## Symptom
An aggregation endpoint calls a downstream service 30 times per request, concurrently, and returns the sum. With 16 users the downstream sees **hundreds of calls in flight** and each request takes well over **a hundred milliseconds**, although one request alone would finish in about 10 ms. The downstream's latency climbs with load; the harness shows `peakInflight`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2433 ref-ms |
| Median allocated | 7 MB |
| Median p99 latency | 254 ref-ms |
| peakInflight | ≤ 61 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.

## Extra credit
Add a 100 ms timeout to `WaitAsync` and return 503 when it expires. What does that do to p99 and to the error rate?
