# L6-04 · Worker counters (false sharing)

## Symptom
Eight threads each increment **their own** counter 5 million times. There is no sharing (each thread only touches its own element) and no lock, yet the run takes **hundreds of milliseconds** and eight threads are no faster than one (or slower). Nothing is contended in the code.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 369 ref-ms |
| Median allocated | 1 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

Needs **at least 8 cores** to show the effect clearly (the result also depends on which cores the threads land on).

## Extra credit
Run with 2, 4 and 8 threads (shared version). At what thread count does the penalty appear on your machine, and does it depend on which cores get used (try `taskset -c 0-3` vs `taskset -c 0,10`)?
