# L06-03 - Solution

## What the profile shows
- **Sampling:** the `if` line. **`perf stat`:** branch-miss rate ~25–50%.

## Root cause
A data-dependent branch on random data: the predictor is wrong about half the time, and each miss flushes the pipeline.

## Fix
Sort once (an O(n log n) cost paid once) so the 60 passes see a perfectly predictable branch. An alternative is a *branchless* formulation (`sum += b & mask`), which removes the branch altogether (extra credit). The right choice depends on how many passes amortise the sort.

## Take-aways
1. **Data can make the same code fast or slow**: a sorted vs unsorted input can differ several-fold with an unchanged loop.
2. Confirm with the branch-miss counter, not the stopwatch alone.
3. Sorting is only worth it if you scan repeatedly; for one pass, prefer branchless code or vectorisation.
4. Newer JITs sometimes convert simple `if`s into conditional moves; the exercise's two-statement body defeats that on purpose. Look at the disassembly (`DOTNET_JitDisasm`) to see what you actually got.

## Extra credit
Replace the body with `sum += b >= 128 ? b : 0` in a scratch copy. Did the JIT make it branchless? How can you tell?

## Go further
Write the branchless version and find the pass count at which sort-once stops being worth it. Then look at the JIT's output for both loops.
