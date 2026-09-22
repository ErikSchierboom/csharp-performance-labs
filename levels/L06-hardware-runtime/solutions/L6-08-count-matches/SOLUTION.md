# L6-08 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** the lambda and LINQ's `Count` loop (per-element delegate call); the fix is ~all memory-bandwidth-limited SIMD.

## Root cause
A scalar loop with one delegate call per element, when a vectorised BCL primitive exists.

## Fix
`((ReadOnlySpan<int>)Data).Count(42)` (`MemoryExtensions.Count`), vectorised and allocation-free. If no primitive fits, `Vector<T>`/`Vector128<T>` are the next tool, after measuring.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 7.5 ms | 0.00 MB |
| after | ≈ 2.9 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Look for a vectorised BCL primitive first**: `Count`, `IndexOf`, `Contains`, `SequenceEqual`, `Sum` (on spans), `Max`… are hand-tuned.
2. LINQ over arrays is convenient but per-element delegate calls prevent inlining and vectorisation; in hot paths use span methods or loops.
3. Once vectorised you're usually memory-bandwidth-bound: the remaining gap to the hardware limit is small.
4. Check what you got: disassembly (`vpcmpeqd`/`vpaddd`) is the proof of vectorisation.

## Go further
Write the loop by hand with `Vector<int>` and compare with `Count`. Then compare with a plain `for` loop over the span (does the JIT vectorise it? (No; it won't auto-vectorise.)).

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- [Akinshin, Pro .NET Benchmarking](https://www.apress.com/us/book/9781484249406)
- [Toub, Performance Improvements in .NET 10 (vectorisation sections)](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/)
