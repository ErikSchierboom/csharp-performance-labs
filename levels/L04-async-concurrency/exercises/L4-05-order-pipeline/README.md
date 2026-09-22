# L4-05 · Order pipeline (unbounded queue)

## Symptom
A fast producer feeds a slower consumer through a queue. Everything is correct and the total time is fine, but **the queue grows to thousands of items** (30 MB of 10 KB buffers here) before the consumer catches up. In production the input never stops, and the same shape ends in an OutOfMemory or a container kill.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 106 ref-ms |
| Median allocated | 73 MB |
| maxQueued | ≤ 76 |

## Note
The harness reports `maxQueued`: the largest number of items waiting in the queue during the run. Each queued item is a 10 KB buffer, so it is also a proxy for peak memory.

## Extra credit
Make the consumer 4× faster. Where does `maxQueued` settle for the *unbounded* version, and why?
