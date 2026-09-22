# L1-04 · Quantity parsing

## Symptom
Cleaning one spreadsheet column of 100,000 cells takes about **half a second**. Roughly half of the cells are
junk (`N/A`, blank, `--`, …) and are simply skipped. The code has no nested loops and no I/O, and it is
"obviously O(n)". Yet 100,000 cells shouldn't take this long.

## Goal
Same parsed values (checksum), and:

| Budget | Value |
|---|---|
| Median time | 100 ref-ms |
| Median allocated | 12 MB |

## Extra experiment
Run the slow version once under a **debugger** (not a profiler, just step through it normally) and compare. The harness
warns you when a debugger is attached. Why does it matter so much for this particular problem?
