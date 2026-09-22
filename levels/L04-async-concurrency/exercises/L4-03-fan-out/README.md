# L4-03 · Fan-out

## Symptom
Fetching 1,000 items from a downstream service, all at once with `Task.WhenAll`, takes **about half a second**, and the downstream ends up with ~1,000 requests in flight. Firing everything immediately feels like it should be the fastest possible approach.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 495 ref-ms |
| Median allocated | 2 MB |
| peakInflight | ≤ 76 |

## Note
The "downstream service" in this exercise gets **slower the more requests are in flight at once** (as most real ones do). That's the point.

## Extra credit
Replace `Parallel.ForEachAsync` with a `SemaphoreSlim`-gated `Task.WhenAll`. Same behaviour? What are the trade-offs?
