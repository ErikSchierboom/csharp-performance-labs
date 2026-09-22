# L1-06 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** `ArraySortHelper`/introsort frames dominate; **tracing:** 6,000 `Sort` calls over lists growing to 6,000.

## Root cause
Re-sorting the whole list on every insertion: total work grows quadratically, and it is done to answer a question (`what's the maximum?`) that needs O(1).

## Fix
Track the maximum incrementally. If you truly need ordered access, use a structure that keeps order (`SortedSet<T>`, a heap/`PriorityQueue`) or sort once at the end.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 204 ms | 0.26 MB |
| after | ≈ 0.1 ms | 0.26 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Maintain the invariant you need, not a stronger one.** Sorted is stronger than 'max'.
2. Work inside a loop multiplies: cost per call × calls × growth.
3. Ask 'what does the caller actually read?' before choosing a structure.

## Go further
Support 'top 10 after each insert' with a `PriorityQueue` or a bounded sorted list. Compare with re-sorting.

## Further reading
- JetBrains dotTrace docs: profiling modes *(title only)*
- [Gregg, The Flame Graph (ACM Queue)](https://queue.acm.org/detail.cfm?id=N2927301)
