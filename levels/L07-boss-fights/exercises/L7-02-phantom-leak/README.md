# L7-02 · Phantom leak (native memory)

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

The harness reports `privateMB`: growth of the **process's private memory** across the run. `kept after full GC` (the managed-heap leak gate) will say **there is no managed leak**. Both facts are true.

## Extra credit
I also tried to build a *managed* version of this exercise (LOH fragmentation from varying-size arrays) and could not make the heap grow beyond the live data on this runtime: the LOH reused the holes efficiently. If you can construct a fragmentation case that does grow (pinned buffers interleaved with garbage are the usual suspect), you have found a new exercise.
