# L4-02 · Latency stats (lock held too long)

## Symptom
Eight workers record 320,000 latency samples into one shared recorder. The work is embarrassingly parallel, yet the run takes about as long as doing it **on one thread**. The CPU meter shows roughly one core busy, not eight.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 73 ref-ms |
| Median allocated | 1 MB |

## Note
Needs **at least 4 cores** to show the effect. The exercise starts its own threads/tasks, so the result doesn't depend on how busy your machine is, but a 2-core box will under-report the improvement.

## Extra credit
Keep the lock but move `Audit` outside it. How much of the win do you get? What's left, and why?
