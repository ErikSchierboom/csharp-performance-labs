# L12-01 - Downstream call

*Phone a Friend*

## Symptom
An endpoint calls a downstream service. **The callee sees a new TCP connection almost every request** (the harness counts distinct connections: `connections`). At scale this becomes port exhaustion, TIME_WAIT pile-ups and TLS handshake CPU.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 8 ref-ms |
| Median allocated | 3 MB |
| Median p99 latency | 1 ref-ms |
| connections | 16 |

> The exercise runs two separate ASP.NET Core servers on loopback **inside the harness process** (`WebRig`): the endpoint under test, and the downstream service it calls. Allocation and CPU include the small constant client cost.
