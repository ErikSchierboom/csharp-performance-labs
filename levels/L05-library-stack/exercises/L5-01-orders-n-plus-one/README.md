# L5-01 · Orders (N+1)

## Symptom
A report totals the orders of 500 customers. It takes ~50 ms here (and far longer against a real database over a network) and sends **hundreds of SQL commands** for what is conceptually one question. Reading the C#, every line looks reasonable: one loop, one query per customer.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 6 ref-ms |
| Median allocated | 2 MB |
| sqlCommands | ≤ 3 |

## Note
The database is an in-memory SQLite shared by the whole run, seeded once. The harness counts the SQL commands your code sends (`sqlCommands`). If your profiler has a SQL/ADO.NET subsystem view (Rider's dotTrace does), open it and compare with the count; otherwise EF's own command logging tells you the same thing.

## Extra credit
Increase to 5,000 customers in a scratch copy. How do the two versions scale?
