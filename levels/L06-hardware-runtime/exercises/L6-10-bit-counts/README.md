# L6-10 · Bit counts (hardware intrinsics)

## Symptom
Counting the set bits in 4 million 64-bit words takes **~60 ms**. The loop is the well-known 'clear the lowest set bit' trick, which is already smarter than testing every bit. The loop's iteration count depends on the data and the branch is hard to predict.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 9 ref-ms |
| Median allocated | 1 MB |

## Extra credit
Run with `DOTNET_EnableHWIntrinsic=0`. What does `PopCount` cost when the hardware path is disabled?
