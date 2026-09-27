# L11-boss - Solution

## Root cause
One defect from three Level 11 exercises.

## Fix
Do the slow external call before renting a connection; `AsSplitQuery()` for the two collections; compute the total from the loaded orders instead of querying per order.

## Take-aways
1. **Defect -> source:** connection held across a slow call = **L11-03**; two collection `Include`s = **L11-02**; a query per order = **L11-01**.
2. Each has its own metric: pool waiters, rows returned, statements per request. Which did you look at first?
3. Fixing the N+1 and the explosion barely helps p99 until the pool hold time is fixed: throughput is capped by size ÷ hold time.

## Extra credit
Which single change moves p99 the most? `sqlCommands` the most?

## Go further
Project straight to a DTO with a server-side aggregate and compare with split queries.
