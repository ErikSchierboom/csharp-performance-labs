# L4-07 · Throttled too tight (concurrency too low)

## Symptom
200 independent calls to an API that answers in 10 ms each take **about 2 seconds**: exactly 200 × 10 ms. The code 'limits concurrency to be polite'. The API's owners say it can take dozens of concurrent calls without slowing down. Nothing is busy: CPU is idle, threads are idle.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 288 ref-ms |
| Median allocated | 2 MB |

## Extra credit
What's the *smallest* degree that finishes in under 200 ms?
