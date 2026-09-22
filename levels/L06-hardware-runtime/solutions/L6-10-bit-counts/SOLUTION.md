# L6-10 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** the inner `while` loop; data-dependent trip count (~32) with mispredicted exits.

## Root cause
A hand-written bit-counting loop where a single hardware instruction exists.

## Fix
`BitOperations.PopCount(w)`: intrinsic, branch-free, constant time.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 56 ms | 0.00 MB |
| after | ≈ 2.3 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Look in `System.Numerics.BitOperations`, `Math`, `MemoryExtensions` and `Vector` before writing tricks.**
2. Hardware intrinsics are exposed in a portable way: the JIT picks the instruction, with a fallback.
3. Data-dependent loops mispredict; constant-time operations don't (L6-03).

## Go further
Sum popcounts over the array with `Vector128`/`Popcnt.X64` intrinsics or `TensorPrimitives.PopCount`. Is it faster still?

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- docs/BENCHMARKDOTNET.md (this repo)
