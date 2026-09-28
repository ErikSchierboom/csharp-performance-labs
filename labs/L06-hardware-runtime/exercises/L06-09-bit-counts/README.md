# L06-09 - Bit counts

*Bit by Bit*

## Symptom
Counting the set bits in 4 million 64-bit words takes **~60 ms**. The loop is the well-known 'clear the lowest set bit' trick, which is already smarter than testing every bit. It's still slow.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 0 MB |
