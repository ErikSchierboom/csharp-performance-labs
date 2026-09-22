# L2-05 · Price lookup

## Symptom
Two million price lookups against a 300-item cache take about **50 ms** and allocate **153 MB**. Almost every call is a cache hit that returns
immediately without waiting for anything. Yet about 80 bytes are allocated per call.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 70 ref-ms |
| Median allocated | 4 MB |

## Note
Time is scaled to your machine; allocation and collection counts are not.

## Extra credit
Two valid fixes exist: `ValueTask<decimal>`, or caching the `Task<decimal>` objects themselves. Try both. Then break the `ValueTask` version by awaiting the same instance twice, and read what the documentation says must never be done with one.
