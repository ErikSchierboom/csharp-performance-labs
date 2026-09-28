# L01-07 - Field reader

*Pick a Column, Any Column*

## Symptom
A configurable report totals a column chosen **by name** over 200,000 items, three times. It takes **~125 ms and allocates tens of MB**, for what should be a few million additions.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 25 ref-ms |
| Median allocated | 1 MB |
