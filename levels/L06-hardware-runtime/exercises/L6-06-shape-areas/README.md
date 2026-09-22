# L6-06 · Shape areas (dispatch)

## Symptom
Summing the area of 1 million shapes, 30 passes, takes **~200 ms**: ~7 ns per virtual call, which is much more than the multiply it performs. The interface is small and the classes are `sealed`. Making them `sealed` didn't help.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 247 ref-ms |
| Median allocated | 1 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

## Extra credit
Change the mix to 95% `Rect`. What does the slow version do, and why? (Think dynamic PGO.)
