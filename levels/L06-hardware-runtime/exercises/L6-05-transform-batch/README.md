# L6-05 · Transform batch (hidden struct copies)

## Symptom
Calling a small method on a large (512-byte) struct stored in a `static readonly` field, 20 million times, costs **~5 ns per call** for what is three multiplications. The field is never modified. The call looks like a plain instance call, so it isn't obvious where a per-call cost could come from.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 105 ref-ms |
| Median allocated | 1 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

## Extra credit
Remove `[MethodImpl(NoInlining)]` from `Score`. Does the difference between the two versions shrink? What does that say about inlining and copy elision?
