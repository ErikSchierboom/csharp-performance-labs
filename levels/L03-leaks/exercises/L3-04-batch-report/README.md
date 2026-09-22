# L3-04 · Batch report (a leak that isn't)

## Symptom
A dashboard shows the process's memory climbing to several hundred MB during a batch, so someone declared a **memory leak** and added `GC.Collect()` after each batch to "keep it down". The batch is now slow, and the harness shows about
**150 gen2 collections per run**. The **kept-after-a-full-GC** figure, though, is essentially zero, both before and after. Your job is to *prove there is no leak* and remove the harmful workaround.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 33 ref-ms |
| Median allocated | 201 MB |
| Median gen2 collections | ≤ 12 |
| Kept after a full GC | ≤ 1 MB |

## Extra credit
Change one batch to *actually* leak (add the `big` array to a static list). What does the kept figure do? Now you have a leak fingerprint to compare against.
