# L04-07 - Partner API calls

*Politeness Has a Price*

## Symptom
200 independent calls to an API that answers in 10 ms each take **about 2 seconds**: exactly 200 x 10 ms. The API's owners say it can take dozens of concurrent calls without slowing down. Nothing is busy: CPU is idle, threads are idle.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 100 ref-ms |
| Median allocated | 1 MB |
