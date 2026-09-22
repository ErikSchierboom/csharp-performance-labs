# L5-02 · Product list (tracking & over-fetching)

## Symptom
Listing the names and prices of active products in three categories takes **~140 ms and allocates ~87 MB**, from a catalogue of only 10,000 rows. The result is a few thousand short rows. Most of the memory and time go on something that never reaches the output.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 25 ref-ms |
| Median allocated | 4 MB |

## Extra credit
Add `.AsNoTracking()` to the original but keep the in-memory filter. Which of the three costs did you remove, and which remain?
