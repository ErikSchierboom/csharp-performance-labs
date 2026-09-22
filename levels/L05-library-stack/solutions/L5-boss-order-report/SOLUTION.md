# L5-boss · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- Four independent Level 5 defects: N+1 queries, tracked and over-fetched entities (an 800-char notes column nobody reads), interpolated log messages for a disabled level, and a file open/write/close per line.

## Root cause
One defect from four Level 5 exercises.

## Fix
One projection with a server-side aggregate (`AsNoTracking`, only the needed columns); no per-order log formatting; one buffered writer for the audit file.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 40 ms | 7.13 MB |
| after | ≈ 0.9 ms | 0.45 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Defect → source:** query per customer = **L5-01**; tracking and loading the notes column = **L5-02**; `LogDebug($"…")` at a disabled level = **L5-06**; `File.AppendAllText` per line = **L5-07**.
2. The **SQL view** and the **allocation view** each find a different subset: did you use both?
3. The checksum includes the audit file length: what does that tell you about the file's content?

## Go further
Write the report to a `Channel`-fed background writer and discuss durability.

## Further reading
- Your own LAB-LOG entries for L5-01, L5-02, L5-06, L5-07
- docs/READING-LIST.md, Level 5 section
