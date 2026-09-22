# L6-01 · Matrix walk (cache locality)

## Symptom
Summing a 4096×4096 grid of integers takes **~100 ms**. The loop does exactly N² additions and allocates nothing; the profiler puts 100% of the time on one line, `sum += Grid[row * N + col]`. There is nothing in that line to optimise.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 36 ref-ms |
| Median allocated | 1 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

## Extra credit
Reduce N to 256 (the grid fits in cache) in a scratch copy. How much does the loop order matter now?
