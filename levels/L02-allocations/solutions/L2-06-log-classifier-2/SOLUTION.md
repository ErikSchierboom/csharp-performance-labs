# L2-06 · Solution

## What the profile shows
> **About the profile section.** I have not captured Rider/dotTrace/dotMemory output for this exercise. What is described is what the code and the harness numbers imply. The harness figures (time, MB, GC counts) were measured. Where your profiler shows something different, trust the profiler and note the difference in `templates/LAB-LOG.md`.

- **dotMemory:** `Match`, `GroupCollection`, `Group[]`, `Capture` arrays and many `string` (each `.Value`, including the three you don't need to keep alive).
- **dotTrace:** the compiled regex runner is now cheap. Most of the remaining time is allocation and GC-related.

## Root cause
Regular expressions are excellent for flexible matching and poor for high-volume parsing of a fixed, simple format: each `Match` produces a graph of objects, and `.Value` produces strings. The cost moved from *constructing* the regex (L1) to *consuming its results*.

## Fix
Parse the fixed grammar by hand over a `ReadOnlySpan<char>`: find the first space (timestamp), read the upper-case level up to the next space, read the service up to `" - "`, and check whether the rest ends with `" status=ddd"`.
Only allocate the two strings and the `LogEntry` that are part of the result (or avoid those too: see Extra credit).
Keep the grammar identical. In particular the regex's lazy `.*?` followed by an optional group at `$` means "the message ends where a trailing ` status=ddd` begins, if there is one".

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before (static compiled regex, part 1's result) | ≈ 43 ms | 89 MB |
| after (span parser; two strings + `LogEntry` per line) | ≈ 6 ms | 10.9 MB |

Absolute times depend on your machine and runtime version; the *ratios* and the allocation numbers should look similar.

## Take-aways
1. **Fixing a cost often reveals the next one.** Regex construction hid regex *results*. Re-profile after every fix.
2. A hand-written parser is faster but is code you now maintain and test. It is the right call for a hot path with a stable format, the wrong call for a flexible one. The checksum here is doing the work a test suite would.
3. The remaining 10.9 MB is not waste in the parser; it is the *result objects*. Whether that matters depends on what consumers need. Changing the result type is an API decision, not a micro-optimisation.
4. `Regex.EnumerateMatches` and `IsMatch` avoid `Match` objects, but they don't give you group values, so they can't replace this parser directly.

## Go further
Return a `readonly record struct` and see if the caller loop can avoid all allocation. Then benchmark against `[GeneratedRegex]` with `EnumerateMatches` for a single group.

## Further reading
- [Best practices for regular expressions in .NET](https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-regex)
- [Regular expression source generators](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators)
- [Toub, All About Span](https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay)
