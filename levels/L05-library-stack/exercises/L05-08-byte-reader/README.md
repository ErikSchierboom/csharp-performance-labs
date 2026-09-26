# L05-08 - Byte reader

*Byte-Sized*

## Symptom
Summing the bytes of a 400 KB file takes **~150 ms**, mostly outside your code, and the CPU is busy in the kernel. Reading 400,000 bytes should take a fraction of a millisecond from memory.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 1 MB |
