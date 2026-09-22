# L6-02 · Particle sweep (pointer chasing)

## Symptom
Visiting 2 million particles and summing five fields takes **~18 ms**: about 9 ns per particle, which is far more than five loads and a few multiplies. The data structure is a plain array of a small class. There is no allocation in the loop.

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
Remove the shuffle in the *slow* version's `Create`. How close does the class array get, and what does that tell you about the allocator?
