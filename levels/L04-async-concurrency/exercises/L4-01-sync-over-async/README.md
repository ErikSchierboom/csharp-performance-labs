# L4-01 · Sync over async

## Symptom
A burst of 200 requests, each needing a 20 ms database lookup, finishes in **hundreds of milliseconds** (the slowest request waits ~0.5 s), yet the CPU is nearly idle the whole time. A single request on its own takes 20 ms. Nothing is computing; requests are *waiting for each other*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 83 ref-ms |
| Median allocated | 2 MB |
| Median p99 latency | 82 ref-ms |

## Note
The harness pins the thread pool's *minimum* thread count to 4 before each run (`Workload.Reset`), so the effect doesn't depend on how many cores you have. It only pins the starting point; the pool can still grow. That is scaffolding, **not** the fix.
The size of the effect varies with runtime version and machine; on the machine this was written on it was roughly 10–25×.

## Extra credit
Predict the slowest request's latency for 400 requests before running it (in a scratch copy). Is the growth linear?
