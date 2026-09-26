# L06-08 - Solution

## What the profile shows

- **Sampling:** all the time in LINQ's `Count` and the lambda, one element at a time.

## Root cause
A scalar loop with one delegate call per element, when a vectorised BCL primitive exists.

## Fix
`((ReadOnlySpan<int>)Data).Count(42)` (`MemoryExtensions.Count`), vectorised and allocation-free. If no primitive fits, `Vector<T>`/`Vector128<T>` are the next tool, after measuring.

## Take-aways
1. **Look for a vectorised BCL primitive first**: `Count`, `IndexOf`, `Contains`, `SequenceEqual`, `Sum` (on spans), `Max`... are hand-tuned.
2. LINQ over arrays is convenient but per-element delegate calls prevent inlining and vectorisation; in hot paths use span methods or loops.
4. Check what you got: disassembly (`vpcmpeqd`/`vpaddd`) is the proof of vectorisation.

## Extra credit
Run the fix with `DOTNET_EnableAVX2=0` (128-bit SSE, 4 ints per compare) and with `DOTNET_EnableHWIntrinsic=0` (no SIMD). What does that tell you about deploying to older or smaller hardware? Then, in a scratch copy, make `Data` 64 times bigger and `Passes` 64 times smaller (the same number of comparisons) and run all three again. Why do they end up so much closer together?

## Go further
Write the loop by hand with `Vector<int>` and compare with `Count`. Then compare with a plain `for` loop over the span (does the JIT vectorise it? (No; it won't auto-vectorise.)).
