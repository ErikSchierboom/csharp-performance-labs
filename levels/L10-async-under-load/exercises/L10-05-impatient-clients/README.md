# L10-05 - Impatient clients

*Talking to an Empty Room*

## Symptom
Clients abandon requests after 30 ms (timeouts, closed tabs), but the server **keeps doing the full 200 ms of work** for every one of them. In production this is capacity wasted on answers nobody will read, and during an incident it makes things worse: slow requests get retried, the abandoned originals keep running, load doubles. The harness reports `stepsAfterAbort`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2170 ref-ms |
| Median allocated | 8 MB |
| stepsAfterAbort | ≤ 1 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.
