# L06-boss - Solution

## What the profile shows
- Three separate hardware/runtime effects; the profile is three flat, unremarkable loops.

## Root cause
One defect from each of three Level 6 exercises.

## Fix
Walk the grid in memory order; make `Reading` a struct so the array holds the data inline; use the vectorised `Count` on a span.

## Take-aways
1. column-major walk = **L06-01**; array of class instances (pointer chasing) = **L06-02**; LINQ `Count` with a lambda = **L06-08**.
2. None of these show as a slow *function*: they show as flat loops. Predict-then-measure is the only way in.

## Go further
Which of the three is limited by memory bandwidth after the fix, and which by instruction throughput? How would you tell?
