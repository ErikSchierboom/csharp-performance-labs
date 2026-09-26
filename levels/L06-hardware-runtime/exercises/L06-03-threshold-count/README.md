# L06-03 - Threshold count

*Over the Threshold*

## Symptom
Counting readings above a threshold over 1 million random bytes, 60 passes, takes **~200–300 ms**: several nanoseconds per element for a compare-and-add. The loop body is about as simple as code gets.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 60 ref-ms |
| Median allocated | 1 MB |
