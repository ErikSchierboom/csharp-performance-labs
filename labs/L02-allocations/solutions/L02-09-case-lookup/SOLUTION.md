# L02-09 - Solution

## What the profile shows
- **Allocations:** a `string` per lookup (≈ 60 bytes) from `ToLowerInvariant`, unless the input is already lower case.

## Root cause
Allocating a normalised copy of the key on each lookup instead of using a case-insensitive comparer.

## Fix
Construct the dictionary with `StringComparer.OrdinalIgnoreCase` and look up with the raw key. (Use `Ordinal*` comparers for identifiers and protocol text; culture-aware comparers are slower and rarely what you want.)

## Take-aways
1. **Don't copy to compare.** Comparers exist so the data doesn't have to change shape.
2. `Ordinal` vs culture-sensitive: choose deliberately (and note `InvariantGlobalization` is set in this repo).
3. `ToLower`/`ToUpper` on user input also has correctness pitfalls (the Turkish-i problem): another reason to use comparers.

## Extra credit
What does the dictionary do for hashing when the comparer is `OrdinalIgnoreCase`? Look at how it avoids allocation.

## Go further
Use `Dictionary<string,T>.GetAlternateLookup<ReadOnlySpan<char>>()` (.NET 9+) to look up spans without creating strings at all.