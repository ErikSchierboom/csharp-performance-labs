# L4-04 · Config cache (factory runs many times)

## Symptom
Eight threads ask a cache for the *same* section at the same moment. The wall-clock time looks fine (the threads run in parallel), but total **CPU time is several times higher than it should be** and the loader shows up 100+ times for 20 distinct sections. The harness reports `factoryCalls` and process CPU time.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 424 ref-ms |
| Median allocated | 2 MB |
| Median CPU time | 459 ref-ms |
| factoryCalls | ≤ 31 |

## Note
Needs **at least 4 cores** to show the effect. The exercise starts its own threads/tasks, so the result doesn't depend on how busy your machine is, but a 2-core box will under-report the improvement.

## Extra credit
Print `factoryCalls` for 2, 8 and 32 threads. What does the number do, and why isn't it always 20 × threads?
