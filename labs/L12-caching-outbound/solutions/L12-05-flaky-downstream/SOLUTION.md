# L12-05 - Solution

## Root cause
Immediate, unbounded-rate retries against an overloaded dependency amplify the load that caused the failures (positive feedback). The extra calls aren't free: a "fails fast" rejection still occupies one of the downstream's limited workers for its full duration, so a flood of retries competes with genuine requests for the same capacity. More load in means everyone - not just the retried requests - waits longer.

## Fix
Bound concurrency toward the dependency (`SemaphoreSlim(15)`, below the downstream's capacity of 20), so it never overloads; then retries are rarely needed. In production add timeouts, capped retries with jittered exponential backoff, and a circuit breaker (`Microsoft.Extensions.Http.Resilience`).

## Take-aways
1. **Retries are load.** Without limits, they turn blips into outages.
2. Prefer prevention (concurrency limits, load shedding) to reaction (retries).
3. If you retry: cap attempts, add exponential backoff with jitter, and use a retry *budget* (e.g. ≤ 10% extra calls).
4. Make retried operations idempotent, and propagate deadlines.

## Extra credit
Raise the gate to 25 (above the downstream's 20). What happens?

## Go further
Add jittered backoff to the *slow* version. How far does that get without a bulkhead?
