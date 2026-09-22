# L10 · Sync over async (in an endpoint)

## Symptom
Under 200 concurrent users, an endpoint whose database call takes 20 ms has a **p99 latency in the hundreds of milliseconds**, and throughput collapses as concurrency rises, but the CPU is almost idle. With a single user it takes 20 ms.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 405 ref-ms |
| Median allocated | 7 MB |
| Median p99 latency | 107 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool is constrained before each run (`Workload.Reset`: minimum 4 threads, and in L10-01 a maximum of 32, standing in for a CPU-limited container; that is scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.

## Extra credit
Increase users to 400. How do the two versions' p99s scale?
