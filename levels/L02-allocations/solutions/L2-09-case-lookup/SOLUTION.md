# L2-09 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocations:** a `string` per lookup (≈ 60 bytes) from `ToLowerInvariant`, unless the input is already lower case.

## Root cause
Allocating a normalised copy of the key on each lookup instead of using a case-insensitive comparer.

## Fix
Construct the dictionary with `StringComparer.OrdinalIgnoreCase` and look up with the raw key. (Use `Ordinal*` comparers for identifiers and protocol text; culture-aware comparers are slower and rarely what you want.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 19 ms | 12.20 MB |
| after | ≈ 11 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Don't copy to compare.** Comparers exist so the data doesn't have to change shape.
2. `Ordinal` vs culture-sensitive: choose deliberately (and note `InvariantGlobalization` is set in this repo).
3. `ToLower`/`ToUpper` on user input also has correctness pitfalls (the Turkish-i problem): another reason to use comparers.

## Go further
Use `Dictionary<string,T>.GetAlternateLookup<ReadOnlySpan<char>>()` (.NET 9+) to look up spans without creating strings at all.

## Further reading
- [Kokosa et al., Pro .NET Memory Management](https://prodotnetmemory.com/)
- [Toub, Performance Improvements in .NET 10](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/)
