# L11 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Three Level 11 defects: a connection held across a third-party call, two sibling collection `Include`s (row explosion), and a query per order (N+1).

## Root cause
One defect from three Level 11 exercises.

## Fix
Do the slow external call before renting a connection; `AsSplitQuery()` for the two collections; compute the total from the loaded orders instead of querying per order.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency | sqlCommands |
|---|---|---|---|---|
| before | ≈ 1438 ms | 238.98 MB | ≈ 148 ms | 8400 |
| after | ≈ 303 ms | 29.34 MB | ≈ 30 ms | 1200 |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** connection held across a slow call = **L11-04**; two collection `Include`s = **L11-03**; a query per order = **L11-01**.
2. Each has its own metric: pool waiters, rows returned, statements per request. Which did you look at first?
3. Fixing the N+1 and the explosion barely helps p99 until the pool hold time is fixed: throughput is capped by size ÷ hold time.

## Go further
Project straight to a DTO with a server-side aggregate and compare with split queries.

## Further reading
- Your own LAB-LOG entries for L11-01, L11-03, L11-04
- docs/READING-LIST.md, Level 11 list
