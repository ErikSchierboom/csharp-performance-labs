# L04-06 - Reference data

*Mostly Harmless*

## Symptom
Reference data is read by eight workers constantly and updated only a handful of times. **Two million reads take longer than they would on a single thread.** Each read is one dictionary lookup.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 20 ref-ms |
| Median allocated | 2 MB |

## Note
Needs **at least 4 cores** to show the effect. The exercise starts its own threads/tasks, so the result doesn't depend on how busy your machine is, but a 2-core box will under-report the improvement.
