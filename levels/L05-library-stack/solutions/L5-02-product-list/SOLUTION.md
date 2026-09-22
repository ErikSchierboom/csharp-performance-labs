# L5-02 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory:** `Product` (10,000×3) and their `string` Descriptions dominate, plus EF's change-tracker entries (`InternalEntityEntry`, snapshots).
- **SQL view:** three `SELECT` of all columns with no `WHERE`.

## Root cause
The filter and the column selection happen in C# after loading everything, and each load goes through the change tracker. Over-fetching rows, over-fetching columns, and paying tracking overhead for read-only data.

## Fix
`AsNoTracking()` + `Where` (translated to SQL) + `Select` to a small projection. Now the database filters, only two columns travel, and EF doesn't track anything.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 141 ms | 87.11 MB |
| after | ≈ 6.2 ms | 1.40 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Push filters to the database and select only the columns you need.**
2. Tracking is for entities you will modify; read-only queries should use `AsNoTracking` (or projections, which aren't tracked).
3. 'Loads everything then `.Where` in memory' looks identical to a server-side filter when a method returns `IEnumerable`: watch for premature `ToList()`.
4. Wide columns (text/blobs) make over-fetching much worse: consider table splitting.

## Go further
Compare `AsNoTracking()` on the *full-entity* query versus the projection, to see the tracking cost separately from the over-fetch cost.

## Further reading
- [Efficient Querying (EF Core)](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Tracking vs. No-Tracking Queries (EF Core)](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
