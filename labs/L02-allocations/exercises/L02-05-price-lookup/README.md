# L02-05 - Price lookup

*Price Check on Aisle Two Million*

## Symptom
Two million price lookups against a 300-item cache take about **50 ms** and allocate **153 MB**. Almost every call is a cache hit that returns
immediately without waiting for anything. Yet about 80 bytes are allocated per call.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 30 ref-ms |
| Median allocated | 0.5 MB |

## Note
Time is scaled to your machine; allocation and collection counts are not.
