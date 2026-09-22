# L1-07 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** `RuntimeType.GetProperty`, `RuntimePropertyInfo.GetValue`/`Invoke`, argument checking; **allocations:** boxed `decimal`/`int`, `object[]`.

## Root cause
Per-item reflection: a property lookup and a boxed, late-bound read for every one of 600,000 reads.

## Fix
Resolve the member once per report (here a `switch` returning a typed `Func<Item, decimal>`; in general `Delegate.CreateDelegate`, compiled expression trees, or a source generator) and use the delegate in the loop.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 11 ms | 15.27 MB |
| after | ≈ 1.9 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Hoist the lookup, keep the read cheap.** Late binding is fine outside hot loops.
2. Reflection allocates (boxing, argument arrays) as well as burning CPU.
3. Modern options: source generators / `UnsafeAccessor` (.NET 8+) give reflection-like flexibility with direct-call speed.

## Go further
Build the getter generically with `Delegate.CreateDelegate` from the `PropertyInfo`. Compare with the `switch` and with compiled expressions.

## Further reading
- JetBrains dotTrace docs: profiling modes *(title only)*
- [Gregg, The Flame Graph (ACM Queue)](https://queue.acm.org/detail.cfm?id=N2927301)
- Microsoft Learn: Reflection and performance; UnsafeAccessor *(title only)*
