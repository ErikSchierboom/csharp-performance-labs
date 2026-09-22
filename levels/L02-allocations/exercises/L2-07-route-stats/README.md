# L2-07 · Route stats

## Symptom
Counting one million requests per (tenant, route, status) takes about **91 ms** and allocates about **79 MB**, but the resulting table has only about 48,000 distinct
entries. Almost all of the allocation is not the table, but something built and thrown away for each *request*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 60 ref-ms |
| Median allocated | 8 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)

## Extra credit
A hand-written `struct` key that overrides `Equals(object)` and `GetHashCode()` but does **not** implement `IEquatable<T>` looks fine and compiles. Try it and measure allocated MB. Then explain what `Dictionary` does with it (I measured ~61 MB, close to the original, instead of ~2 MB).
