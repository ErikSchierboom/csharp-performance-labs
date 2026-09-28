# L06-08 - Count matches

*Counting Sheep*

## Symptom
A monitor re-scans a buffer of 256,000 readings 400 times, counting how many equal 42. That's 100 million comparisons, and it takes **~31 ms**: about 0.3 ns per element. The code is a single readable LINQ expression. The buffer is 1 MB, so it sits in the CPU cache and memory speed isn't the limit.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 8 ref-ms |
| Median allocated | 1 MB |
