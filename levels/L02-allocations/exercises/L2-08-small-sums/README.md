# L2-08 · Small sums (interface enumeration)

## Symptom
Summing a 24-element `List<int>` two million times allocates **~50 MB** and is slower than the arithmetic suggests. Nothing in `Sum` allocates: it is a `foreach` and an addition. Yet each call produces garbage.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 43 ref-ms |
| Median allocated | 1 MB |

## Extra credit
What happens if you pass `Values.AsEnumerable()` vs `(IReadOnlyList<int>)Values`? Predict the allocations.
