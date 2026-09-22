# L0-01 · Solution

## What the profile shows
> The harness numbers below were measured. The top self-time frame, `System.Buffer.BulkMoveWithWriteBarrierBatch`, was observed in a Rider sampling profile (Release build, profile mode); the rest of the call-tree shape is what the code implies.

- **Sampling:** nearly all self time is in one runtime routine, `System.Buffer.BulkMoveWithWriteBarrierBatch(ref Byte, ref Byte, UIntPtr)`, reached from `Array.Copy` in `List<T>.Insert`, from `Feed.Build`. It is not a plain `memmove`: see the root cause.
  `FeedItem` construction and the checksum loop are noise.
- **Allocation:** ≈ 2 MB, the 60,000 `FeedItem` objects. The GC is idle, so allocation is not the problem.

## Root cause
`List<T>.Insert(0, x)` shifts every existing element right by one. The k-th insert moves k elements, so 60,000 inserts move about 1.8 billion element slots: O(n²) work in total.
Doubling the count roughly quadruples the time.

**Why the hot frame is not a plain `memmove`.** `FeedItem` is a `record`, a reference type, so the list's array holds GC references. For an array whose element type contains GC references, `Array.Copy` does not use `Memmove`: it calls `Buffer.BulkMoveWithWriteBarrier` so the garbage collector is told about the moved references [[108]](../../../../docs/READING-LIST.md#ref108), [[109]](../../../../docs/READING-LIST.md#ref109). Blocks bigger than 16 KB go through `BulkMoveWithWriteBarrierBatch`, which copies in 16 KB chunks and gives the GC a chance to run between chunks. Each full `Insert` here moves about 480 KB (60,000 references × 8 bytes), so every call takes that path.

## Fix
Append (`Add`, O(1) amortised) in arrival order, then `Reverse()` once at the end: O(n) total. Also pre-size the list (`new List<FeedItem>(count)`).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before (`Insert(0, …)`) | ≈ 187 ms | 2.37 MB |
| after (`Add` + one `Reverse`) | ≈ 0.6 ms | 1.83 MB |

## Take-aways
1. **Total cost = cost per call × calls.** Each `Insert` is microseconds and looks harmless; the *count* and the growing size make it quadratic.
2. A profile that is all one data-movement routine (here `BulkMoveWithWriteBarrierBatch`) and no allocation tells you the category (CPU, data movement) before you have read any code. The routine's *name* tells you more: it moves references, which is a hint about the element type.
3. Prediction check (see the worked LAB-LOG): 60,000 → 600,000 items should cost about 100× (10² for n²), not 10×.

## Break your own fix
The fix is wrong when readers need the feed **between** inserts (you can't reverse "at the end" if someone reads it after every insert). Then use a structure with O(1) front insertion:
a ring buffer / `LinkedList<T>` (poor cache locality, so measure it), or keep the list in arrival order and *index from the back* when reading.
At a count of 5 the original is perfectly fine: never optimise what the profile doesn't show.

## Further reading
- Microsoft Learn: `List<T>` API remarks (the array-backed growth strategy) *(title only)*
- [[107]](../../../../docs/READING-LIST.md#ref107): amortised analysis of dynamic arrays.
- Bakhvalov, *Performance Analysis and Tuning on Modern CPUs*, on why memmove of contiguous data is still cheap per byte but not free at scale ([free PDF](https://book.easyperf.net/perf_book))
