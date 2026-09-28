# L05-boss - Solution

## What the profile shows
- Four independent Lab 5 defects: N+1 queries, tracked and over-fetched entities (an 800-char notes column nobody reads), interpolated log messages for a disabled level, and a file open/write/close per line.

## Root cause
One defect from four Lab 5 exercises.

## Fix
One projection with a server-side aggregate (`AsNoTracking`, only the needed columns); no per-order log formatting; one buffered writer for the audit file.

## Take-aways
1. **Defect -> source:** query per customer = **L05-01**; tracking and loading the notes column = **L05-02**; `LogDebug($"...")` at a disabled level = **L05-06**; `File.AppendAllText` per line = **L05-07**.
2. The **SQL view** and the **allocation view** each find a different subset: did you use both?
3. The checksum includes the audit file length: what does that tell you about the file's content?

## Extra credit
Which fix removes the most allocation? The most time?

## Go further
Write the report to a `Channel`-fed background writer and discuss durability.
