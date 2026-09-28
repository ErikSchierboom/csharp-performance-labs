# L01-03 - Log classifier

*Read the Logs, They Said*

## Symptom
Parsing 40,000 log lines takes about **0.25 seconds** and allocates roughly **380 MB**. The parsing logic is
a few lines per log line. That shouldn't cost this much. The GC is busy (gen0 and gen1 counts
are non-zero in the harness output).

## Goal
Same parsed results (checksum), and:

| Budget | Value      |
|---|------------|
| Median time | 147 ref-ms |
| Median allocated | 100 MB     |
