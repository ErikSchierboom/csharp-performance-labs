# L09-boss - Solution

## What the profile shows
- Four independent L9 defects on one request path.

## Root cause
One defect from four Level 9 exercises.

## Fix
Static `Regex` and `[LoggerMessage]` in the middleware; the tax table as a real singleton; deserialise straight from the body stream; build the receipt once and write it once.

## Take-aways
1. **Defect -> source:** per-request `Regex` and interpolated log = **L09-02**; `Transient` per request = **L09-04**; body -> string = **L09-05**; flush per line = **L09-03**.
2. Each layer of the pipeline had its own signature in the allocation view: which types pointed at which?
3. Measure bytes per request before and after each fix: which gave the largest drop?

## Extra credit
Which single defect causes the gen2 collections?

## Go further
Compare the result with the L09-01 floor: how many bytes per request are left above an empty endpoint?
