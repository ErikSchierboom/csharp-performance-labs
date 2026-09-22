# L10 · Fire and forget (unbounded background work)

## Symptom
An endpoint accepts a job and hands it to `Task.Run` so the response is quick. Under a burst of 300 requests, **hundreds of jobs run at once**, the thread pool is flooded with blocked threads, and the *HTTP requests themselves* slow down (p99 climbs) because the same pool serves both. The jobs finish, but the server was unresponsive on the way. The harness reports `peakConcurrentJobs`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1439 ref-ms |
| Median allocated | 3 MB |
| Median p99 latency | 7 ref-ms |
| peakConcurrentJobs | ≤ 7 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.

## Extra credit
Make the jobs `async` (await a 5 ms delay) instead of blocking. Do you still need the bounded queue?
