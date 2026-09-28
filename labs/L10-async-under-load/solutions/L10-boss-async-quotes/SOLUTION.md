# L10-boss - Solution

## Root cause
One defect from three Lab 10 exercises.

## Fix
`await` everywhere (no blocked pool threads), and one `SemaphoreSlim` shared by all requests in front of the downstream.

## Take-aways
1. **Defect -> source:** `Thread.Sleep` in an `async` handler = **L10-02**; `.Result` = **L10-01**; unbounded fan-out x concurrency = **L10-03**.
2. Blocking defects starve the pool (visible in pool counters); the fan-out overloads the downstream (visible as in-flight count): different tools found different defects.
3. Which did you fix first, and what did the next profile look like?

## Extra credit
Which fix moves p99 the most? `peakInflight` the most?

## Go further
Add a request deadline (`CancellationToken`) and 503 when the permit wait exceeds it (L10-05).
