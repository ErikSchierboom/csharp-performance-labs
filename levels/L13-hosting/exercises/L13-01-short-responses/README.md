# L13-01 - Short responses

*Short and Not So Sweet*

## Symptom
**Each request costs a fresh TCP connection** (and, in production, a TLS handshake). 1,200 requests open ~1,200 connections (the harness counts `connections`). The endpoint returns a constant string, so all of the time is connection setup and teardown.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 33 ref-ms |
| Median allocated | 8 MB |
| Median p99 latency | 6 ref-ms |
| connections | ≤ 25 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Allocation and CPU include the small constant client cost.
