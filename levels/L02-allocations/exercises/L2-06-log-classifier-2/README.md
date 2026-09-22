# L2-06 · Log classifier, part 2

## Symptom
This is where **L1-03 left off**: the regex is built once and is compiled, so it is no longer the *time* problem. Parsing 100,000 log lines still
takes about **43 ms** and allocates about **89 MB**, which is roughly 900 bytes of garbage per line, for a result that is one small object. (The 100,000 input strings are generated
once, before measurement, and are not counted.)

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 30 ref-ms |
| Median allocated | 16 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)

## Extra credit
The budget allows the two small strings per entry (`Level`, `Service`) and the `LogEntry` object. Take it further: return a `readonly record struct` with an enum for the level
and an interned/cached service name, and see how close to zero allocation you can get without changing the checksum.
