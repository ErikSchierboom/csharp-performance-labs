# L1-06 · Leaderboard (sorting inside a loop)

## Symptom
A live leaderboard records 6,000 scores and, after each one, reads the current leader. It takes **about 100 ms** and burns CPU in `Sort`. Each individual call looks fine ("keep it sorted so the top is first"), and the profiler blames the runtime's sort routine, not your code.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 2 MB |

## Extra credit
Double the count to 12,000. Predict the ratio for the slow version, then measure.
