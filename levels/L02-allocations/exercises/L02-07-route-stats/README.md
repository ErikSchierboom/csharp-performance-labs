# L02-07 - Route stats

*Counting Is Hard*

## Symptom
Counting one million requests per (tenant, route, status) takes about **91 ms** and allocates about **79 MB**, but the resulting table has only about 48,000 distinct
entries. Almost all of the allocation is not the table, but something built and thrown away for each *request*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 20 ref-ms |
| Median allocated | 2 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)
