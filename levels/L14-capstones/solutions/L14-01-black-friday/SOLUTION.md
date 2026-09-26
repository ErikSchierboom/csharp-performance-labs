# L14-01 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Metrics:** `externalCalls` several × the number of products, `giveUps` > 0, pool saturated; p99 in seconds.

## Root cause
Three stacked defects: a non-atomic cache (concurrent loads per key), a connection held across a third-party call, and immediate unbounded retries against a dependency that fails fast when overloaded.

## Fix
Single-flight per key (`Lazy<Task>` map), a bulkhead in front of the third party (12 < its capacity of 15) and no immediate retries, and the connection rented only for the database step. Result: one external call per product, zero give-ups, short pool holds.

## Take-aways
1. **Failure amplifiers compose.** A cache stampede, a slow dependency and retries multiply each other; fix them together or they mask each other.
2. The order that works: stop the fan-in (single-flight), bound the outbound concurrency (bulkhead), hold scarce resources briefly.
3. Draw the request path and put a *count* on each arrow: the anomalous ratio (calls per request) tells you where to look.
4. Your post-mortem should list which defect hid which, and which metric would have caught each in advance.

## Extra credit
Which single fix gives the largest improvement in p99? The largest in `externalCalls`? Are they the same fix?

## Go further
Add a circuit breaker and a 200 ms deadline, and decide what the page shows when the price is unavailable (stale price, 'unavailable', or 503).
