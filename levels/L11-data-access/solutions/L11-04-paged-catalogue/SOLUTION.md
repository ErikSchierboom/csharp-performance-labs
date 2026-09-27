# L11-04 - Solution

## What the profile shows

- **Retained memory:** thousands of `Product` + snapshots + strings, rooted from the static context's change tracker.

## Root cause
A singleton/static tracking `DbContext` accumulates every loaded entity (and needs a lock because it isn't thread-safe).

## Fix
One short-lived context per request/unit of work (`AddDbContext` scoped, or pooled with `AddDbContextPool`) and `AsNoTracking` for read-only queries.

## Take-aways
1. **`DbContext` = unit of work, short-lived, not thread-safe.** Never register it as a singleton.
2. The change tracker is a cache you didn't ask for: it's why long-lived contexts grow.
3. Locks around a shared context hide a design error and serialise your server.
4. This is L3 (retention) inside an ASP.NET app: the same tools (snapshot, dominators) find it.

## Extra credit
Keep the shared context but call `ChangeTracker.Clear()` after each request. Does that fix the retention? What's still wrong?

## Go further
Register `AddDbContextPool` and compare per-request allocation with `new` contexts.
