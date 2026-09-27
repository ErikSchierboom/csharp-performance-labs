# L14-02 - OOM-killed at 3 a.m.

*Night Shift*

## Symptom
In production the container is **OOM-killed every few hours**, at night when the batch traffic arrives. No single request is slow, and nothing looks wrong request-by-request. The memory graph tells a different story: it's a staircase that only goes up, and a full GC barely brings it back down.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 42 ref-ms |
| Median allocated | 10 MB |
| Median gen2 collections | ≤ 2 |
| Median p99 latency | 6 ref-ms |
| Kept after a full GC | ≤ 2 MB |

## Capstone rules
Symptom only; several defects, each hiding the next. Write the post-mortem (`templates/POSTMORTEM.md`) **before** reading the solution. Hints are generic on purpose.
