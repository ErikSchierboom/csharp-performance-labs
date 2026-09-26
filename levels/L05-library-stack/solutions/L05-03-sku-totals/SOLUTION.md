# L05-03 - Solution

## What the profile shows

- **Sampling:** self time in the SQLite native library (a full table scan per query); EF and your code barely register.
- **Query plan:** `SCAN Lines` before; `SEARCH Lines USING INDEX IX_Lines_Sku (Sku=?)` after.

## Root cause
The `WHERE Sku = @p` query has no supporting index, so every execution is a full table scan: O(rows) per query instead of O(log rows + matches).

## Fix
Add an index on `Sku` (`modelBuilder.Entity<Line>().HasIndex(l => l.Sku)`), created with the schema. In production you'd add it with a migration, and check the plan.

## Take-aways
1. **A slow query with a fast-looking C# line is a plan problem.** Read the query plan.
2. Index the columns you filter, join and sort on; but every index costs writes and space, so add them for measured queries.
3. Correlating profiler time (native DB frames) with `EXPLAIN` is the skill: the profiler tells you *where*, the plan tells you *why*.
4. Data volume matters: this is invisible at 1,000 rows and painful at 200,000.

## Go further
Add a composite index for `WHERE Sku = @p AND Cents > @min`. How does the plan change, and why does column order matter?
