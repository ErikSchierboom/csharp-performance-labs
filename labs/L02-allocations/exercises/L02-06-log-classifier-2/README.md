# L02-06 - Log classifier, part 2

*Log Rolling*

## Symptom
This is where **L01-03 left off**: the regex is built once and is compiled, so it is no longer the *time* problem. Parsing 100,000 log lines still
takes about **50 ms** and allocates about **89 MB**, which is roughly 900 bytes of garbage per line, for a result that is one small object. (The 100,000 input strings are generated
once, before measurement, and are not counted.)

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 10 ref-ms |
| Median allocated | 12 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)