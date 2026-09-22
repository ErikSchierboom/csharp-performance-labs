# L2-03 · Page renderer

## Symptom
Rendering 6,000 pages takes about **120 ms** and allocates **570 MB**, and the harness reports **~187 gen2 collections** in a single
run. That is almost one full collection for every 32 pages. Each page only needs a scratch canvas of about 100 KB.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 80 ref-ms |
| Median allocated | 8 MB |
| Median gen2 collections | ≤ 2 |

## Note
Time is scaled to your machine; allocation and collection counts are not.

## Extra credit
**Trap:** the first fix most people try produces `WRONG RESULT` (exit code 2). Make sure you understand *why* before you fix it. Then answer: why does `ArrayPool<byte>.Shared.Rent(100_000)` return an array with `Length` 131072, and what would have gone wrong if the code had used `canvas.Length` instead of the constant?
