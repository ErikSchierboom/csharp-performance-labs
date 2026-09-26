# L06-09 - Solution

## What the profile shows
- **Sampling:** the inner `while` loop; data-dependent trip count (~32) with mispredicted exits.

## Root cause
A hand-written bit-counting loop where a single hardware instruction exists.

## Fix
`BitOperations.PopCount(w)`: intrinsic, branch-free, constant time.

## Take-aways
1. **Look in `System.Numerics.BitOperations`, `Math`, `MemoryExtensions` and `Vector` before writing tricks.**
2. Hardware intrinsics are exposed in a portable way: the JIT picks the instruction, with a fallback.
3. Data-dependent loops mispredict; constant-time operations don't (L06-03).

## Extra credit
Run with `DOTNET_EnableHWIntrinsic=0`. What does `PopCount` cost when the hardware path is disabled?

## Go further
Sum popcounts over the array with `Vector128`/`Popcnt.X64` intrinsics or `TensorPrimitives.PopCount`. Is it faster still?
