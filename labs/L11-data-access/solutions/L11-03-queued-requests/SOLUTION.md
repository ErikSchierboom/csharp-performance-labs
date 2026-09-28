# L11-03 - Solution

## What the profile shows

- **Pool:** 8/8 in use, many waiters; each connection spends ~97% of its hold time idle, waiting on the third party.

## Root cause
Connections are held across an unrelated slow call, so pool capacity ÷ hold time caps throughput far below what the database could serve.

## Fix
Reorder so the connection is rented after the slow call and released immediately after the query; keep rent scopes as small as possible.

## Take-aways
1. **Hold scarce resources briefly.** Connections, locks and semaphores should wrap only the work that needs them.
2. Pool exhaustion is a throughput ceiling (size ÷ hold time), not necessarily a database problem.
3. Also seen with transactions held across HTTP calls or user think-time.
4. The same reasoning applies to thread-pool threads, `HttpClient` connections and semaphore permits.

## Extra credit
Compute the throughput ceiling for pool size 8, hold 31 ms; then for hold 1 ms. Compare with what the harness measures.

## Go further
Add a timeout to `RentAsync` and return 503 quickly when the pool is exhausted. What do you gain and lose?
