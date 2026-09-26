# L10-04 - Currency conversion

*Exchange Rates May Vary*

## Symptom
A currency-conversion endpoint fetches the rate from a remote service (10 ms). With 50 users the **whole endpoint is limited to ~100 requests per second and the p99 is a full second or more**, even though there are only four distinct currencies and the rates barely change.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 12 ref-ms |
| Median allocated | 3 MB |
| Median p99 latency | 6 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.
