# L1-02 · Solution

## What the profile shows
Only time was over budget, so memory tools were the wrong choice. In a sampling profile the top frames are
the lambda inside `Any`, `string.Equals` with `OrdinalIgnoreCase`, and `Enumerable.Any`, all called from
`ContactImporter.Import`. Nothing is individually slow; there are just an enormous number of calls
(order of 10⁸ comparisons).

## Root cause
`seen` is a `List<string>` searched linearly for every row. Work ≈ rows × (average size of `seen`) → **O(n²)**.
It hid at small sizes: 2,000 rows is ~100× fewer comparisons than 20,000.

## Fix
`HashSet<string>(StringComparer.OrdinalIgnoreCase)` and `if (!seen.Add(email)) continue;`: one hash lookup per
row, and `Add` doubles as the "seen it?" test (no separate `Contains` + `Add`).
The comparer keeps the exact semantics of the old `string.Equals(..., OrdinalIgnoreCase)`.

## Measured (1-core sandbox, .NET 8, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 1,090–1,120 ms | 11.0 MB |
| after | ≈ 15 ms | 8.9 MB |

## Take-aways
1. Which budget failed told you which tool to use. CPU-bound with flat memory → sampling.
2. Complexity bugs are invisible until N grows. When you review code, ask "what is N in production?"
3. The old code also allocated a closure per row (the lambda captures `email`). That went away as a side effect.

## Go further / trade-offs
- A `HashSet` costs memory (~tens of bytes per entry) and has a per-lookup hashing cost. For N ≈ 10, a
  `List` scan is often *faster*. Measure at the realistic N, not the theoretical one.
- If you needed insertion order *and* lookups, you'd keep the result `List` plus the `HashSet`, as here.

## Further reading
- Microsoft Learn: Collections and data structures (choosing a collection) *(title only)*
- Cormen et al., *Introduction to Algorithms*: hash tables *(title only)*
- Watson, *Writing High-Performance .NET Code*: the chapter on collections and algorithms
