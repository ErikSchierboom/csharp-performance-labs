# L01-boss - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- Five independent Lab 1 defects: a `Regex` built per line, exceptions for bad amounts, a lazy query enumerated four times (re-running the parse each time), a linear customer lookup per row, and quadratic string building.

## Root cause
One defect from each of the five Lab 1 exercises.

## Fix
Static `Regex`; `decimal.TryParse`; `ToList()` once; `Dictionary` lookup; `StringBuilder`.

## Take-aways
1. **Defect > source:** `new Regex` per line = **L01-03**; `try/catch` around `Parse` = **L01-04**; a lazy `Select/Where` enumerated by `Any`/`Count`/`Sum`/`foreach` = **L01-05**; `FirstOrDefault` per row = **L01-02**; `report +=` in a loop = **L01-01**.
2. Which did you find first, and which last? Fixing the largest changes what the profile shows next.
3. Did you check your fix against the checksum after each step?

## Extra credit
Which single fix removes the most time? Which removes the most allocation?

## Go further
Add a sixth defect of your own from L01-06/L01-07 and see whether a colleague finds it.
