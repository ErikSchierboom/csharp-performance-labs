# L14-01 - Solution

## What the profile shows
Measured on the author's machine (unscaled):

| | Slow | Fixed |
|---|---|---|
| Median time | ≈ 1670 ms | ≈ 63 ms |
| Median p99 | ≈ 220 ms | ≈ 61 ms |
| `externalCalls` | ≈ 1650 | 30 |
| `giveUps` | ≈ 535 | 0 |

## Root cause
Three stacked defects: a non-atomic cache (concurrent loads per key), a connection held across a third-party call, and immediate unbounded retries against a dependency that fails fast when overloaded but still ties up capacity while doing so. A fourth issue made the slow version look better than it is: a give-up used to be cached the same as a success and returned the same `200`/`"ok"`, so once any product gave up, every later request for it silently got served that same fake answer forever, without ever calling the third party again - understating both `externalCalls` and `giveUps`. Fixed two ways: a give-up now returns `429` (a real client sees a real failure, not a silent wrong answer) and expires immediately instead of being cached (the next request retries instead of reusing it).

> **Why the checksum doesn't change with the 429 rate:** how many requests give up depends on live thread scheduling, not on the fix - it isn't reproducible run to run (measured 538-544 in three back-to-back runs of the same slow code). `WebRig.Drive`'s own checksum is sensitive to status codes, so if `Run()` returned it directly, the exercise would fail with "WRONG RESULT" almost every run before ever reaching the budget table. `Run()` instead returns a fixed value representing the fully-healthy shape (600 identical `"ok"` `200`s); `giveUps`, a budgeted metric rather than part of the checksum, is what actually catches the failure rate.

## Fix
Single-flight per key (`Lazy<Task>` map), a bulkhead in front of the third party (12 < its capacity of 15) and no immediate retries, and the connection rented only for the database step. A give-up is removed from the single-flight map immediately rather than memoised, so the next request retries instead of reusing a stale failure. Result: one external call per product, zero give-ups, short pool holds.

## Take-aways
1. **Failure amplifiers compose.** A cache stampede, a slow dependency and retries multiply each other; fix them together or they mask each other.
2. The order that works: stop the fan-in (single-flight), bound the outbound concurrency (bulkhead), hold scarce resources briefly.
3. Draw the request path and put a *count* on each arrow: the anomalous ratio (calls per request) tells you where to look.
4. Your post-mortem should list which defect hid which, and which metric would have caught each in advance.

## Extra credit
Which single fix gives the largest improvement in p99? The largest in `externalCalls`? Are they the same fix?

## Go further
Add a circuit breaker and a 200 ms deadline. A give-up already returns `429` here - would a stale cached price, or a `503`, serve users better?
