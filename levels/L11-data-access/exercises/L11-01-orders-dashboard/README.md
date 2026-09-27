# L11-01 - Orders dashboard

*Dashboard Confessional*

## Symptom
A dashboard endpoint totals the orders of 20 customers. Under load its p99 is many times a single query's cost. *From the outside* (logs, traces) you'd see hundreds of near-identical statements per second. The harness counts them (`sqlCommands`).

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 94 ref-ms |
| Median allocated | 62 MB |
| Median p99 latency | 12 ref-ms |
| sqlCommands | ≤ 601 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
