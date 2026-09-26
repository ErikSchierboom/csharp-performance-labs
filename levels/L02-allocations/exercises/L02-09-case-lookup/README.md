# L02-09 - Case lookup

*Case Closed*

## Symptom
A case-insensitive dictionary lookup that should allocate nothing doesn't: 400,000 lookups allocate **~20 MB** of short strings and spend a large share of the time creating them.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 43 ref-ms |
| Median allocated | 1 MB |
