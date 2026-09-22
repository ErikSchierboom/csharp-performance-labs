# L1-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Five independent Level 1 defects: a `Regex` built per line, exceptions for bad amounts, a lazy query enumerated four times (re-running the parse each time), a linear customer lookup per row, and quadratic string building.

## Root cause
One defect from each of the five Level 1 exercises.

## Fix
Static `Regex`; `decimal.TryParse`; `ToList()` once; `Dictionary` lookup; `StringBuilder`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 50 ms | 490.40 MB |
| after | ≈ 2.1 ms | 3.30 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** `new Regex` per line = **L1-03**; `try/catch` around `Parse` = **L1-04**; a lazy `Select/Where` enumerated by `Any`/`Count`/`Sum`/`foreach` = **L1-05**; `FirstOrDefault` per row = **L1-02**; `report +=` in a loop = **L1-01**.
2. Which did you find first, and which last? Fixing the largest changes what the profile shows next.
3. Did you check your fix against the checksum after each step?

## Go further
Add a sixth defect of your own from L1-06/L1-07 and see whether a colleague finds it.

## Further reading
- Your own LAB-LOG entries for L1-01 to L1-05
- docs/READING-LIST.md, Level 1 section
