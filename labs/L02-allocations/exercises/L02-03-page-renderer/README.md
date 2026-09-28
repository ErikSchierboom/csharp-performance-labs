# L02-03 - Page renderer

*Page After Page*

## Symptom
Rendering 6,000 pages takes about **120 ms** and allocates **570 MB**, and the harness reports **~187 gen2 collections** in a single
run. That is almost one full collection for every 32 pages. Each page only needs a scratch canvas of about 100 KB.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 20 ref-ms |
| Median allocated | 0 MB |
| Median gen2 collections | ≤ 2 |

## Note
Time is scaled to your machine; allocation and collection counts are not.