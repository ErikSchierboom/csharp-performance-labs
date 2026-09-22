# L11 · DbContext lifetime (a leak in disguise)

## Symptom
A long-lived `DbContext` shared by the whole application (guarded by a lock because it isn't thread-safe) serves a paged catalogue. After 400 requests, about **30 MB is still reachable after a full GC**, and it grows with every distinct row ever read. Requests also serialise behind the lock. Nothing 'leaks' in the usual sense: the context is doing its job of *tracking every entity it has ever loaded*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 163 ref-ms |
| Median allocated | 166 MB |
| Median p99 latency | 9 ref-ms |
| Kept after a full GC | ≤ 1 MB |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Databases are in-memory SQLite, seeded once. Allocation and CPU include the small constant client cost.

## Extra credit
Keep the shared context but call `ChangeTracker.Clear()` after each request. Does that fix the retention? What's still wrong?
