# L11 · Two collection Includes (row explosion)

## Symptom
Loading one customer with their 20 orders and 20 addresses returns **400 rows** from the database (20 × 20) instead of 40, and per-request allocation and latency are much higher than the data (40 small entities) justifies. Both collections are loaded 'the recommended way', with `Include`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 127 ref-ms |
| Median allocated | 73 MB |
| Median p99 latency | 15 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.

## Extra credit
Increase both collections to 100 items. How do the two versions scale?
