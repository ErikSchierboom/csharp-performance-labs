# L7-03 · Works on my laptop (GC configuration)

## Symptom
The service is fine on a developer laptop, but on the 20-core build server (a stand-in for a big production node) the same workload, which has almost no long-lived data, uses **over 300 MB of committed memory** and the container's memory graph looks alarming. The code has no leak: kept-after-full-GC is tiny. Only the *configuration* differs between environments.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 740 ref-ms |
| Median allocated | 11582 MB |
| committedMB | ≤ 102 |
| workingSetMB | ≤ 154 |

## Boss fight rules
- **Symptom only.** Hints are deliberately generic. Write the post-mortem (`templates/POSTMORTEM.md`) *before* you read the solution.

This one is fixed in the **project file / runtime configuration**, not in the C#. It needs **at least 8 cores** (Server GC creates one heap per core, and the bloat scales with the core count). The harness prints the GC mode and core count at the top of every run.

## Extra credit
Remove the `DynamicAdaptationMode` line (leave Server GC on) on a runtime that supports DATAS (.NET 9+). What happens to committed memory and to the gen0 count, and what is the cost?
