# L07-03 - Works on my laptop

*Twenty Cores, One Problem*

## Symptom
The service looked fine on a developer laptop, but on the 20-core build server the same workload, which has almost no long-lived data, uses **over 300 MB of committed memory** and the container's memory graph looks alarming. The code has no leak: kept-after-full-GC is tiny.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 300 ref-ms |
| Median allocated | 5000 MB |
| committedMB | ≤ 25 |
| workingSetMB | ≤ 150 |

## Boss fight rules
- **Symptom only.** Hints are deliberately generic. Write the post-mortem (`templates/POSTMORTEM.md`) *before* you read the solution.
