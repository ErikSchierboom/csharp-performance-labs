# L14 · OOM-killed at 3 a.m. (capstone)

## Symptom
In production the container is **OOM-killed every few hours**, at night when the batch traffic arrives. In the harness: after 1,200 requests, **over 100 MB is still reachable after a full GC**, and there are **many gen2 collections**. No single request is slow. The memory graph is a staircase that only goes up.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 42 ref-ms |
| Median allocated | 10 MB |
| Median gen2 collections | ≤ 2 |
| Median p99 latency | 6 ref-ms |
| Kept after a full GC | ≤ 2 MB |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Allocation and CPU include the small constant client cost.

## Capstone rules
Symptom only; several defects, each hiding the next. Write the post-mortem (`templates/POSTMORTEM.md`) **before** reading the solution. Hints are generic on purpose.

## Extra credit
What eviction policy fits this workload: LRU/size, TTL, or both, and how would you size the limit?
