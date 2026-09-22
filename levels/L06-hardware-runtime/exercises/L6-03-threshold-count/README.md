# L6-03 · Threshold count (branch misprediction)

## Symptom
Counting readings above a threshold over 1 million random bytes, 60 passes, takes **~200–300 ms**: several nanoseconds per element for a compare-and-add. The code is a `foreach` and an `if`. Deleting the `if` (in a scratch copy, adding unconditionally) makes it far faster.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 195 ref-ms |
| Median allocated | 3 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

## Extra credit
Replace the body with `sum += b >= 128 ? b : 0` in a scratch copy. Did the JIT make it branchless? How can you tell?
