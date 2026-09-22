# L4-06 · Reference data (read-mostly cache)

## Symptom
Reference data is read by eight workers constantly and updated only a handful of times. Every read takes a `lock`. **Two million reads take longer than they would on a single thread.** The critical section is one dictionary lookup, so it's hard to believe the lock is the problem.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 34 ref-ms |
| Median allocated | 5 MB |

## Note
Needs **at least 4 cores** to show the effect. The exercise starts its own threads/tasks, so the result doesn't depend on how busy your machine is, but a 2-core box will under-report the improvement.

## Extra credit
Try `ReaderWriterLockSlim` instead. Does it beat the `lock`? Beat copy-on-write? Explain the ordering you observe.
