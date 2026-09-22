# L1-07 · Field reader (reflection in a hot loop)

## Symptom
A configurable report totals a column chosen **by name** over 200,000 items, three times. It takes **~150 ms and allocates tens of MB**, for what should be a few million additions. The profile is full of `RuntimeType`, `PropertyInfo` and `MethodBase.Invoke` frames.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 14 ref-ms |
| Median allocated | 1 MB |

## Extra credit
Cache the `PropertyInfo` but keep calling `GetValue`. How much of the win is the lookup and how much is the invoke?
