# L02-08 - Solution

## What the profile shows
- **Allocations:** `List<int>+Enumerator` boxed, ~2,000,000 instances (one per call).

## Root cause
Boxing of `List<T>`'s struct enumerator when a list is enumerated through `IEnumerable<T>`, plus interface calls per element.

## Fix
Take `ReadOnlySpan<int>` (or `List<int>`/`int[]` when you need the concrete type). No enumerator object, no dispatch, and the JIT can unroll/vectorise.

## Take-aways
1. **`IEnumerable<T>` parameters are convenient and cost a boxed enumerator per call** on hot paths.
2. Spans (or concrete collection types) avoid both boxing and virtual calls.
3. Public APIs that must accept 'anything' can offer overloads: `ReadOnlySpan<T>` for the hot path, `IEnumerable<T>` for the general case.
4. LINQ over `IEnumerable<T>` has the same cost profile (L02-02).

## Extra credit
What happens if you pass `Values.AsEnumerable()` vs `(IReadOnlyList<int>)Values`? Predict the allocations.

## Go further
Add overloads for `int[]` and `IEnumerable<int>`; check which one each call site binds to.