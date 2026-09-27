# L10-05 - Impatient clients

*Talking to an Empty Room*

## Symptom
Most clients abandon their request after 30 ms (timeouts, closed tabs), but the server **keeps doing the full 200 ms of work** for every one of them. The server can only do 4 requests at a time (think of a database pool or a downstream limit), so that wasted work isn't free: abandoned requests take slots, and the clients who *are* still waiting for an answer queue behind them. During an incident it gets worse: slow requests get retried, the abandoned originals keep running, load doubles.

The load is 100 requests from 20 users. Every fourth request comes from a patient client that waits for its answer; the rest give up after 30 ms. The harness reports how long the patient clients waited (p99), how many requests were inside the server at once (`peakInFlight`), and how many work steps ran after the client had gone (`stepsAfterAbort`).

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time (until the server is idle again) | 2000 ms |
| p99 latency of the requests that completed | 1500 ms |
| Median allocated | 8 MB |
| peakInFlight | ≤ 25 |
| stepsAfterAbort | 0 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). Time is dominated by fixed waits, so the time budgets are not machine-scaled. The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.
