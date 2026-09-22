# L5-01 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **SQL view / EF logging:** ~501 commands: `SELECT … FROM Orders WHERE CustomerId = @p` × 500 plus one customers query.
- **Sampling:** time inside EF's query pipeline (compile/cache lookup, command creation, materialisation), repeated 500×.

## Root cause
The classic **N+1**: one query to get N parents, then N queries (one per parent) to get their children. Cost is N × per-query overhead, which is tiny locally and enormous against a networked database.

## Fix
Ask the database once: project to `{ Id, Total = c.Orders.Sum(...) }` so the aggregate is computed server-side in a single query. Alternatives: `Include(c => c.Orders)` (loads all order rows), or `GroupBy` on `Orders`. Use `AsNoTracking()` because nothing is updated.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | sqlCommands |
|---|---|---|---|
| before | ≈ 49 ms | 6.04 MB | 501 |
| after | ≈ 0.9 ms | 0.16 MB | 1 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Count the statements, not just the time.** N+1 hides because each query is fast.
2. Loops that call the database are suspect; ask whether one set-based query can replace them.
3. `Include` avoids N+1 but loads entire rows; a projection loads only what you need (often best).
4. Prevention: log or count commands per request in tests (this harness's `sqlCommands` metric) and fail when it exceeds a budget.

## Go further
Implement it with `Include` and with `GroupBy`; compare `sqlCommands`, allocations and time. Which loads the most data?

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- Kleppmann, Designing Data-Intensive Applications (storage & queries) *(title only)*
