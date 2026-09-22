# L2-08 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocations:** `List<int>+Enumerator` boxed, ~2,000,000 instances (one per call).

## Root cause
Boxing of `List<T>`'s struct enumerator when a list is enumerated through `IEnumerable<T>`, plus interface calls per element.

## Fix
Take `ReadOnlySpan<int>` (or `List<int>`/`int[]` when you need the concrete type). No enumerator object, no dispatch, and the JIT can unroll/vectorise.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 54 ms | 76.26 MB |
| after | ≈ 11 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **`IEnumerable<T>` parameters are convenient and cost a boxed enumerator per call** on hot paths.
2. Spans (or concrete collection types) avoid both boxing and virtual calls.
3. Public APIs that must accept 'anything' can offer overloads: `ReadOnlySpan<T>` for the hot path, `IEnumerable<T>` for the general case.
4. LINQ over `IEnumerable<T>` has the same cost profile (L2-02).

## Go further
Add overloads for `int[]` and `IEnumerable<int>`; check which one each call site binds to.

## Further reading
- [Kokosa et al., Pro .NET Memory Management](https://prodotnetmemory.com/)
- [Toub, Performance Improvements in .NET 10](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/)
