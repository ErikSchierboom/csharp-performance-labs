# L11-boss - Orders service

*Query Storm*

## Symptom
A customer-detail endpoint returns an order total. Under 32 users its **p99 is many times a single request** and the database logs show **~20 statements per request**, plus queries returning **hundreds of rows** for a customer with only 40 related records. The connection pool is saturated though the database is barely busy.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 350 ref-ms |
| Median allocated | 35 MB |
| Median p99 latency | 40 ref-ms |
| sqlCommands | ≤ 3 |

## Final boss fight
The **final boss** of its lab: a disguised combination of that lab's defects with **no per-defect hints**. Profile, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
