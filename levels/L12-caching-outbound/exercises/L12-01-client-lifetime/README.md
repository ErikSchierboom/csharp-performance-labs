# L12 · HttpClient created per request

## Symptom
An endpoint calls another service (here: the same server's `/downstream`). It creates and disposes an `HttpClient` per request, as the docs' `using` habit suggests. Each request pays a new TCP connection; **the callee sees a new connection almost every request** (the harness counts distinct connections: `connections`). At scale this becomes port exhaustion, TIME_WAIT pile-ups and TLS handshake CPU.

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

## Extra credit
Use a `static readonly HttpClient` without `PooledConnectionLifetime`. What breaks when the callee's DNS changes?
