# L11-02 - Customer profile

*Twenty Times Twenty*

## Symptom
Loading one customer with their 20 orders and 20 addresses returns **400 rows** from the database (20 x 20) instead of ~40, and per-request allocation and latency are much higher than the data (40 small entities) justifies. The query looks like the textbook example. The harness reports the raw row count as `rowsPerRequest`; EF folds the duplicated columns back into 20 distinct `Order`s and 20 distinct `Address`es either way, so the *materialised* object graph looks the same size and won't show you this - only the row count does.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 50 ref-ms |
| Median allocated | 35 MB |
| Median p99 latency | 5 ref-ms |
| rowsPerRequest | 41 |
| commandsPerRequest | < 4 |

> The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.
