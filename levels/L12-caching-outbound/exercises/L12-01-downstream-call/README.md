# L12-01 - Downstream call

*Phone a Friend*

## Symptom
An endpoint calls another service (here: the same server's `/downstream`). **The callee sees a new TCP connection almost every request** (the harness counts distinct connections: `connections`). At scale this becomes port exhaustion, TIME_WAIT pile-ups and TLS handshake CPU.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 24 ref-ms |
| Median allocated | 6 MB |
| Median p99 latency | 6 ref-ms |
| connections | ≤ 25 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
