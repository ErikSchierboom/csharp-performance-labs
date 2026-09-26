# L05-02 - Product list

*Window Shopping*

## Symptom
Listing the names and prices of active products in three categories takes **~180 ms and allocates ~86 MB**, from a catalogue of only 10,000 rows. The result is a few thousand short rows. Most of the memory and time go on something that never reaches the output.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 10 ref-ms |
| Median allocated | 2 MB |
