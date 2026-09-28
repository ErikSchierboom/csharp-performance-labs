# L06-05 - Transform batch

*Heavy Lifting*

## Symptom
Calling a small transform method 20 million times costs **~5 ns per call** for what is three multiplications. The call looks like a plain instance call, so it isn't obvious where a per-call cost could come from.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 40 ref-ms |
| Median allocated | 0 MB |
