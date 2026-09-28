# L05-07 - Audit log

*Paper Trail*

## Symptom
Writing 20,000 short audit lines takes **~45 ms** on this machine (and longer on a real disk or network drive), although the total data is only about 500 KB. The CPU is not busy. Most of the time is in the operating system.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 2 ref-ms |
| Median allocated | 2 MB |
