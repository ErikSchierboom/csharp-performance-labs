# L01-03 - Solution

## What the profile shows
Under `LineParser.Parse` the time is dominated by **constructing** the regex (pattern parsing, code
generation, character-class setup, constructor frames) rather than by **matching** (interpreter/scan frames).
The allocation profile shows ~390 MB of throwaway objects, mostly from that construction work.

## Root cause
`new Regex(pattern)` runs on every call. Parsing the pattern and building the matching program is far more
expensive than matching one short line, and the pattern never changes.

## Fix
Build it once: `static readonly Regex`. (Regex instances are thread-safe for matching, so sharing is fine.)

| variant | time | allocated |
|---|---|---|
| before (`new Regex` per call) | ≈ 690–735 ms | 377.6 MB |
| static readonly, interpreted | ≈ 153 ms | 45.2 MB |
| static `Regex.Match(line, pattern)` (internal cache) | ≈ 154 ms | 45.2 MB |
| `[GeneratedRegex]` (source-generated) | ≈ 122 ms | 45.2 MB |
| static readonly + `RegexOptions.Compiled` (**shipped in the solution**) | ≈ 65 ms | 45.2 MB |

Results are pattern- and runtime-dependent: `Compiled` won here; don't assume it always does. Measure yours.

## Take-aways
1. **Warm-up hides one-time costs.** `RegexOptions.Compiled` and `[GeneratedRegex]` trade startup cost for
   throughput. In a long-running service that's a bargain; in a short-lived CLI it may be a loss.
   `[GeneratedRegex]` also moves the cost to build time and is trimming/AOT friendly.
2. The static `Regex.Match(input, pattern)` overload works because of an internal cache, but that cache is small
   (15 entries by default), adds a lookup per call, and silently falls off a cliff if you use many patterns.
3. Three variants, three different numbers: this is why the budget is a *gate*, not a leaderboard.

## Extra credit
After you pass, try three different fixes and compare them.
Also: what's still allocating after your fix, and why?

## Go further
The remaining 45 MB is `Match`/`Group` objects and the strings created by `.Value`, plus the test data itself.
That's a Lab 2 exercise: parse with `IsMatch`/`EnumerateMatches`, `ValueMatch`, or hand-written `Span<char>`
parsing and see how close to zero-allocation you can get.
