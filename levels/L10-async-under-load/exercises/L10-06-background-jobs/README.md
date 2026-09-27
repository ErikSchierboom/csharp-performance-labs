# L10-06 - Background jobs

*Take a Number*

## Symptom
An endpoint accepts a job and runs it in the background so the response is quick. Under a burst of 300 requests, **hundreds of jobs run at once**, the HTTP requests slow down (p99 climbs). The jobs finish, but the server was unresponsive on the way. The harness reports `peakJobs`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1439 ref-ms |
| Median allocated | 3 MB |
| Median p99 latency | 7 ref-ms |
| peakConcurrentJobs | ≤ 7 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.
