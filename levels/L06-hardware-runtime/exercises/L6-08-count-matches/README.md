# L6-08 · Count matches (vectorisation)

## Symptom
Counting how many of 16 million integers equal 42 takes **~7.5 ms**: about 0.5 ns per element. The code is a single readable LINQ expression. The memory bandwidth of the machine could feed this loop several times faster.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 11 ref-ms |
| Median allocated | 1 MB |

## Predict first
From Level 6 on: **write your prediction in `templates/LAB-LOG.md` before you run anything**: which version is faster, by roughly how much, and *why*. Then measure. Being right for the wrong reason doesn't count.
After you pass, confirm with BenchmarkDotNet and/or `perf stat` (see [docs/PROFILING-GUIDE.md](../../../../docs/PROFILING-GUIDE.md#testing-a-hypothesis)).

## Extra credit
Run the fix with `DOTNET_EnableAVX2=0` (or `DOTNET_EnableAVX=0`). How does the result change, and what does that tell you about deploying to older hardware?
