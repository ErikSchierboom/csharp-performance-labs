# L2-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Several independent costs: string building by concatenation, a linear search with a closure per line, `Split`/`Trim`/`ToUpper` garbage, and an `ArrayList` of boxed doubles.

## Root cause
Four defects from earlier levels stacked in one method.

## Fix
`StringBuilder` for the manifest; a `Dictionary` keyed by carrier code; `List<double>`; span slicing plus a stack buffer for the upper-cased code instead of `Split`/`Trim`/`ToUpperInvariant`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 33 ms | 149.69 MB |
| after | ≈ 1.2 ms | 0.89 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** `manifest +=` in a loop = **L1-01**; `Carriers.FirstOrDefault(...)` per line = **L1-02** (and the closure it allocates = **L2-02**); `Split`/`Trim`/`ToUpperInvariant` garbage = **L2-02**; `ArrayList` of `double` = **L2-01** (boxing).
2. Did you find all four? Which did you find *last*, and why?
3. Fixing the biggest first changes the profile: the remaining costs become visible only after it is gone.

## Go further
Rewrite so the line parsing produces a `readonly record struct` instead of parts. Where does the remaining allocation come from?

## Further reading
- Your own LAB-LOG entries for L1-01, L1-02, L2-01, L2-02
- docs/READING-LIST.md, Level 1–2 sections
