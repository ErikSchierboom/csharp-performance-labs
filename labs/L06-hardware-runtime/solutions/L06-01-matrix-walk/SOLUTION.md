# L06-01 - Solution

## What the profile shows

- **Sampling:** 100% self time on the one summation line, an unhelpful but honest result: the cost is *waiting for memory*, invisible as instructions.
- **`perf stat`:** many more `cache-misses` and far lower instructions-per-cycle for the column-major order.

## Root cause
The array is laid out row by row, but the loops visit it column by column. Each access lands on a new cache line, so the CPU stalls on memory almost every iteration; the ALU work is trivial.

## Fix
Swap the loop order so the inner loop walks consecutive memory (`row` outer, `col` inner). Same additions, same result.

## Take-aways
1. **Memory access pattern often matters more than instruction count.** Same work, different order, several times faster.
2. A profiler shows the line but not the stall: form the hypothesis, then confirm with counters (`perf stat`).
3. Row-major (C#, C, Java): iterate the *last* index fastest. Column-major languages (Fortran, MATLAB) are the opposite.
4. The gap grows with data size: the array must exceed the cache for the effect to be dramatic.

## Extra credit
Reduce N to 256 (the grid fits in cache) in a scratch copy. How much does the loop order matter now?

## Go further
Try a blocked (tiled) traversal for a *transpose*, where both reads and writes can't be sequential. What tile size works best on your CPU?
