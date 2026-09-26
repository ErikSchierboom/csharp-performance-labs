# L07-02 - Phantom leak

*The Heap Is Innocent*

## Symptom
A service that processes images through a native library shows a **flat managed heap** (no growth in `gc-heap-size`), the leak gate in this harness reports **nothing retained**, and yet the process's memory climbs by about **150 MB per batch** until the container is killed. Heap snapshots show nothing unusual: there is nothing on the managed side to find.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 6 ref-ms |
| Median allocated | 2 MB |
| privateMB | ≤ 1 |
| Kept after a full GC | ≤ 1 MB |

## Boss fight rules
- **Symptom only.** There may be several defects and fixing one can expose the next; re-measure after every change.
- Hints are deliberately generic. Use `templates/POSTMORTEM.md` and write the post-mortem *before* you read the solution.