# L06-04 - Worker counters

*Eight Is Not Enough*

## Symptom
Eight threads each increment **their own** counter 5 million times. There is no sharing (each thread only touches its own element) and no lock, yet the run takes **hundreds of milliseconds** and eight threads are no faster than one (or slower). Nothing is contended in the code.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 120 ref-ms |
| Median allocated | 1 MB |

Needs **at least 8 cores** to show the effect clearly (the result also depends on which cores the threads land on).
