# L01-05 - Solution

## What the profile shows
- **Sampling:** time sits in `ComputeScore` (the `Math.Sqrt` loop). Perfectly reasonable frame, perfectly
  reasonable call stack; nothing looks broken. This is why sampling alone doesn't reveal it.
- **Tracing:** `ComputeScore` is called roughly **3x per active customer** (plus 1 for `Any`). The *call count*
  is the clue.

## Root cause
`scored` is a **deferred query**, not a collection. Each terminal operation re-runs the whole
`Where → Select` pipeline, including the expensive selector: `Count()` (yes, even `Count`: the selector runs
to preserve side effects), `Sum(...)`, and `OrderByDescending(...).First()` are three full passes, and `Any()` adds one more evaluation.

## Fix
Materialise once: `.ToList()`, then use `Count` (the property), `Sum`, and the ordering on the list.

The ~2.9x speed-up matches the ~3 redundant evaluations almost exactly; that arithmetic is a good sanity
check that you found the *whole* problem.

## Take-aways
1. **Choose the profiler mode for the question.** "Where is time spent?" → sampling. "How many times did this run?"
   → tracing. Tracing adds overhead and distorts absolute times, so use it for counts, then confirm with sampling.
2. Static analysis helps: Rider/ReSharper's *Possible multiple enumeration* and analyzer CA1851 flag this.
3. `ToList()` isn't free (allocation, memory held). For a single pass you can skip it entirely and compute
   count/sum/top in one `foreach`.

## Go further
Write the single-pass version and compare. Then check: does it still match the checksum? (Floating-point
sum order matters: keep the iteration order the same.)
