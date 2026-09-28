# L02-08 - Small sums

*Small Change*

## Symptom
Summing a 24-element list two million times allocates **~50 MB** and is slower than the arithmetic suggests. The result is one number per call, yet each call produces garbage.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 15 ref-ms |
| Median allocated | 0 MB |
