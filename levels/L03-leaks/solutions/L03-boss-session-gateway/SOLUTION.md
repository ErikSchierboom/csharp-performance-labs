# L03-boss - Solution

## What the profile shows
- Three independent retention roots: a long-lived event's delegate list, an unbounded profile dictionary, and a callback list whose closures capture a scratch array.

## Root cause
One defect from three Level 3 exercises.

## Fix
Unsubscribe (`IDisposable`); bound the profile cache (oldest-first eviction, well above the 50-entry lookback); capture only the needed value in the callback.

## Take-aways
1. **Defect > source:** views never unsubscribing from `Presence` = **L03-01**; an ever-growing `Dictionary` = **L03-02**; closures capturing a big local = **L03-06**.
2. In a snapshot compare, each root has its own retention path: did you find all three by looking at *paths*, not just sizes?
3. Which of the three was largest? Which was hardest to spot?

## Extra credit
Break your own fix: what happens if the lookback grows to 500 entries?

## Go further
Register the callbacks with an unsubscribe token and remove them when the session ends.
