# L02-boss - Solution

## What the profile shows
- Several independent costs: string building by concatenation, a linear search with a closure per line, `Split`/`Trim`/`ToUpper` garbage, and an `ArrayList` of boxed doubles.

## Root cause
Four defects from earlier labs stacked in one method.

## Fix
`StringBuilder` for the manifest; a `Dictionary` keyed by carrier code; `List<double>`; span slicing plus a stack buffer for the upper-cased code instead of `Split`/`Trim`/`ToUpperInvariant`.

## Take-aways
1. **Defect -> source:** `manifest +=` in a loop = **L01-01**; `Carriers.FirstOrDefault(...)` per line = **L01-02** (and the closure it allocates = **L02-02**); `Split`/`Trim`/`ToUpperInvariant` garbage = **L02-02**; `ArrayList` of `double` = **L02-01** (boxing).
2. Did you find all four? Which did you find *last*, and why?
3. Fixing the biggest first changes the profile: the remaining costs become visible only after it is gone.

## Extra credit
Which of the four fixes bought the most time? The most allocation? Are they the same one?

## Go further
Rewrite so the line parsing produces a `readonly record struct` instead of parts. Where does the remaining allocation come from?
