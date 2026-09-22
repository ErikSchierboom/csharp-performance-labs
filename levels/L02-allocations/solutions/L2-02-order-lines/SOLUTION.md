# L2-02 · Solution

## What the profile shows
> **About the profile section.** I have not captured Rider/dotTrace/dotMemory output for this exercise. What is described is what the code and the harness numbers imply. The harness figures (time, MB, GC counts) were measured. Where your profiler shows something different, trust the profiler and note the difference in `templates/LAB-LOG.md`.

- **dotMemory:** allocations spread over `string[]`, `string`, `Enumerable+SelectArrayIterator`, `<>c__DisplayClass` (the closure), and delegate instances. No single culprit; that is what death by a thousand cuts looks like.
- **dotTrace:** `String.Split`, `String.Trim`, `String.ToLowerInvariant` and the LINQ iterators sit at the top by self time.

## Root cause
Parsing by cutting a string into new strings. `Split` allocates an array plus a string per field; `Trim`, `ToUpperInvariant` and `ToLowerInvariant` allocate again;
the LINQ chain allocates iterators, and its lambda captures `disabledFlags`, so it also allocates a closure and delegates on every call. Per line that's about 530 bytes for one small struct result.

## Fix
Slice, don't copy. Parse over `ReadOnlySpan<char>` with `IndexOf` and slicing, compare with `Equals(..., StringComparison.OrdinalIgnoreCase)` instead of lowercasing first, and use `int.Parse`/`decimal.Parse` overloads that take spans.
Hoist the disabled-flags set out of the per-line path: turn it into a `LineFlags` mask **once** and apply it with `flags &= ~disabled`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 50 ms | 106 MB |
| after (span parser + flags mask) | ≈ 24 ms | 0.00 MB |

Absolute times depend on your machine and runtime version; the *ratios* and the allocation numbers should look similar.

## Take-aways
1. **Zero allocation is achievable for parsing** when the result is a struct and you never need the intermediate strings.
2. Two different classes of fix here: *don't copy* (spans) and *don't rebuild what doesn't change* (the mask, computed once, not per line).
3. `static` lambdas (`static x => ...`) make the compiler refuse captures, which is a cheap way to keep closure allocations out of hot code.
4. The span version is longer and harder to read. Spend that complexity only where the profiler says the parse is hot.

## Go further
Try `MemoryExtensions.Split` for the general case (check what your target framework offers). Also try `string.Create` and `SearchValues<char>`. Does either beat the hand-written scanner?

## Further reading
- [Toub, All About Span: Exploring a New .NET Mainstay (MSDN Magazine, 2018)](https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay)
- [Sitnik, Span](https://adamsitnik.com/Span/)
- [Teplyakov, Unusual ways of boosting up app performance: lambdas and LINQs](https://blog.jetbrains.com/dotnet/2014/07/24/unusual-ways-of-boosting-up-app-performance-lambdas-and-linqs/)
