# L10-03 - Solution

## What the profile shows
- **Metric:** `peakInflight` ≈ 480 (slow) vs ≤ 40 (fix). **Downstream latency:** climbs sharply with in-flight count.

## Root cause
Unbounded fan-out multiplied by concurrent requests overloads the shared downstream, which slows every call: latency is driven by the *product* of the two concurrency levels.

## Fix
A `SemaphoreSlim(40)` shared across requests gates calls into the downstream. Requests queue briefly instead of overwhelming it.

## Take-aways
1. **Bound concurrency at the shared resource**, not per request: total in-flight = sum over requests.
2. This is L04-03 at request scale; the same 'sweet spot' logic.
3. A bulkhead per dependency also stops one slow dependency from consuming all your capacity.
4. Also consider a timeout and load shedding: waiting forever is its own failure mode.

## Extra credit
Add a 100 ms timeout to `WaitAsync` and return 503 when it expires. What does that do to p99 and to the error rate?

## Go further
Sweep the permit count (5 to 200) and plot p99. Then replace the semaphore with `Parallel.ForEachAsync` per request and explain why it doesn't fix the total.
