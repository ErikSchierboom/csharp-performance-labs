# L04-03 - Bulk fetch

*Release the Kraken*

## Symptom
Fetching 1,000 items from a downstream service takes **about half a second**, and the downstream ends up with ~1,000 requests in flight. The downstream answers a single request quickly, and the code was written to be as fast as possible.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 150 ref-ms |
| Median allocated | 1 MB |
| peakInflight | ≤ 50 |

## Note
The "downstream service" in this exercise gets **slower the more requests are in flight at once** (as most real ones do). That's the point.
