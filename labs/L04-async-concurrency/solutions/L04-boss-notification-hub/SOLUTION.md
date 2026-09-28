# L04-boss - Solution

## What the profile shows
- Three independent defects in three stages.

## Root cause
One defect from each of three earlier labs, in three separate stages of a single batch.

## Fix
Unsubscribe inboxes (dispose); do the pure work outside the lock and use an atomic add; bound the queue.

## Take-aways
1. **Defect -> source:** inboxes never unsubscribe from a long-lived bus = **L03-01**; expensive work inside a `lock` = **L04-02**; unbounded producer/consumer queue = **L04-05**.
2. Final bosses test *recognition*: which signature (retained memory, contention/idle threads, queue depth) pointed at which stage?

## Go further
Order the three by how much each contributes to the time budget, then to memory. Are they different orders?
