# L06-06 - Shape areas

*Shape Up*

## Symptom
Summing the area of 1 million shapes, 30 passes, takes **~200 ms**: ~7 ns per shape, which is much more than the multiply it performs. The shape types are small and simple.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 80 ref-ms |
| Median allocated | 1 MB |
