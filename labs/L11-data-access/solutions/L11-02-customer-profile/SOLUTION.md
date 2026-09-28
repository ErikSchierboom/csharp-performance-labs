# L11-02 - Solution

## What the profile shows
- **SQL:** one query with two joins returning 400 rows; after the fix, three queries returning 1 + 20 + 20 = 41 rows. **Metric:** `rowsPerRequest` 400 -> 41.

## Root cause
Two collection navigations in one JOIN multiply rows (Cartesian product), inflating data transfer, materialisation and memory.

## Fix
`AsSplitQuery()` (extra round trips but far less data), or project to a DTO with aggregates. Split queries are not free: consider consistency and round trips against a networked database.

## Take-aways
1. **Multiple collection Includes multiply rows.** EF warns about it for a reason.
2. Compare rows returned, not just query count: the materialised `Customer.Orders`/`Addresses` collections are the same 20+20 either way, since EF de-duplicates the repeated parent columns when it builds the object graph. Only a row count taken below EF (e.g. wrapping the `DbDataReader`, as `RowCounter` does here) exposes the cartesian blow-up.
3. Projection (`Select`) usually beats `Include` for read models.

## Extra credit
Increase both collections to 100 items. How do the two versions scale?

## Go further
Project to `{ Total = c.Orders.Sum(...), Addresses = c.Addresses.Count() }` and compare with split queries.
