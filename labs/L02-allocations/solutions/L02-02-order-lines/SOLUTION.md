# L02-02 - Solution

## What the profile shows

- **allocations:** spread over `string[]`, `string`, `Enumerable+SelectArrayIterator`, `<>c__DisplayClass` (the closure), and delegate instances. No single culprit; that is what death by a thousand cuts looks like.
- **traces:** `String.Split`, `String.Trim`, `String.ToLowerInvariant` and the LINQ iterators sit at the top by self time.

## Root cause
Parsing by cutting a string into new strings. `Split` allocates an array plus a string per field; `Trim`, `ToUpperInvariant` and `ToLowerInvariant` allocate again;
the LINQ chain allocates iterators, and its lambda captures `disabledFlags`, so it also allocates a closure and delegates on every call. Per line that's about 530 bytes for one small struct result.

## Fix
Slice, don't copy. Convert the line `string` to a `ReadOnlySpan<char>` and use `Split()` to iterate over the parts.
Use the `int.TryParse`/`decimal.TryParse`/`Enum.TryParse<T>` overloads that take a `ReadOnlySpan<char>` and use the `ignoreCase: true` parameter to prevent lowercasing and trimming first.
Hoist the disabled-flags set out of the per-line path: turn it into a `LineFlags` mask **once** and apply it with `flags &= ~disabled`.

## Take-aways
1. **Zero allocation is achievable for parsing** when the result is a struct, and you never need the intermediate strings.
2. Two different classes of fix here: *don't copy* (spans) and *don't rebuild what doesn't change* (the mask, computed once, not per line).
3. `static` lambdas (`static x => ...`) make the compiler refuse captures, which is a cheap way to keep closure allocations out of hot code.
4. The span version is longer and harder to read. Spend that complexity only where the profiler says the parse is hot.

## Extra credit
Your parser must still treat `" eu "`, `"APAC"`, `"Gift"`, `" fragile"` and unknown flags the same way as before, and reject lines
with a wrong field count. Add three edge-case lines to the input in a scratch copy and check your version and the original agree. What about an empty flags field?

## Go further
Try `MemoryExtensions.Split` for the general case (check what your target framework offers). Also try `string.Create` and `SearchValues<char>`. Does either beat the hand-written scanner?
