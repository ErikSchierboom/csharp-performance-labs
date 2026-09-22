# L2-09 · Case lookup (ToLower as a key)

## Symptom
A case-insensitive lookup normalises every query with `ToLowerInvariant()` before hitting a dictionary. 400,000 lookups allocate **~20 MB** of short strings and spend a large share of the time creating them.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 43 ref-ms |
| Median allocated | 1 MB |

## Extra credit
What does the dictionary do for hashing when the comparer is `OrdinalIgnoreCase`? Look at how it avoids allocation.
