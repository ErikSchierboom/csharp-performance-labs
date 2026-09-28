# L02-01 - Sensor metrics

*Sensor and Sensibility*

## Symptom
Summarising 400,000 sensor readings (sum, max, median, and a histogram) takes about **90 ms** and allocates about
**45 MB**, although the input is an `int[]` that already exists and the result is five numbers. The harness shows the GC is
not even busy (0 to 1 collections), yet the number of bytes allocated is far larger than the answer needs.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 25 ref-ms |
| Median allocated | 5 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)
