# L01-02 - Contact import

*Contact Sport*

## Symptom
Importing 20,000 contact rows (about 12,000 distinct people, with repeats and inconsistent casing) takes
**over a second**. One CPU core is pinned; memory looks fine. Yesterday's import of 2,000 rows took a few
milliseconds, and nobody thought about it.

## Goal
Keep the exact same results (first occurrence wins, email compared case-insensitively) and pass:

| Budget | Value     |
|---|-----------|
| Median time | 59 ref-ms |
| Median allocated | 16 MB     |

## Rules
Same as always: edit `Workload.cs` only, hypothesis in `templates/LAB-LOG.md` before changing code, hints one at a time.
Note the interesting part of this exercise: the **allocation** number is *not* the problem here. Which
budget failed tells you which kind of profiler to reach for.
