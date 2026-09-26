# L06-01 - Matrix walk

*Grid Lock*

## Symptom
Summing a 4096×4096 grid of integers takes **~100 ms**. The loop does exactly N^2 additions and allocates nothing. There is nothing to optimise?

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 10 ref-ms |
| Median allocated | 1 MB |
