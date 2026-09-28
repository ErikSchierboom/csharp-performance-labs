# L01-01 - Solution

## What the profile shows
- Sampling: self time is concentrated in string concatenation and the memory copy underneath it
  (`String.Concat` > `Buffer.Memmove`). `FormatRow`, the "expensive-looking" method, is a small slice.
- Harness output: ~1.27 GB allocated to produce a file of a few hundred KB, and **389 gen0/gen1/gen2 GCs**
  (the same number three times: every GC was a full, gen2 collection).

## Root cause
`report += row` creates a brand-new string and copies the whole existing report into it, every iteration.
Copy cost per row grows with the report, so total work is **O(n^2)**: 5,000 rows ≈ 1.2 GB of copying.
Once the string passes ~85,000 bytes it lands on the **Large Object Heap**, and LOH allocation pressure
triggers gen2 collections: that's the 389 full GCs.

## Fix
One `StringBuilder`, pre-sized (`orders.Count * 64 + 64`), appending fields directly (no per-row temp string).
`string.Join`, or `string.Create`, or writing straight to a `TextWriter`/stream would also work.

## Take-aways
1. **Self time vs. total time.** The method that *looks* costly (`FormatRow`) wasn't; the cheap-looking `+=` was.
2. Time and allocations are two views of the same problem. The allocation number is also a *deterministic*
   regression test: it doesn't wobble like wall-clock time.
3. `a + b + c` in one expression is fine (the compiler emits one `Concat`). It's `+=` **in a loop** that hurts.
4. The 85,000-byte LOH threshold is worth remembering; you'll meet it again in Lab 2 and Lab 7.

## Go further
- Remaining allocations: the per-row `ToString("F2")` calls. Use `ISpanFormattable.TryFormat` into a
  `Span<char>` and drop them (a Lab 2 technique).
- The output is culture-sensitive in a normal app; use `CultureInfo.InvariantCulture` for machine-readable CSV.
