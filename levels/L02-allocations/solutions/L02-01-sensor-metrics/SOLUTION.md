# L02-01 - Solution

## What the profile shows

- **allocations:** expect `System.Int32` (boxed) to be the top type by object count: about three boxes per reading,
  24 bytes each, plus the `object[]` arrays that `ArrayList` and `Hashtable` grow into.
- **tracing:** time is spread across `ArrayList.Add`, `Hashtable.set_Item`/`get_Item`, and `ArrayList.Sort` (through
  `Comparer.Default` and `IComparable.CompareTo`). Nothing is individually "slow".

## Root cause
`ArrayList` and `Hashtable` are non-generic: they store `object`. Every `Add(int)` **boxes** (allocates a heap object per value);
every `(int)` cast **unboxes**. The histogram boxes twice per reading: once for the key on lookup and once for the value on store.
That is roughly three allocations per reading. Sorting an `ArrayList` compares through `IComparable` on boxed values instead of comparing
`int`s directly.

## Fix
Use the generic collections: `List<int>`, `Dictionary<int,int>`, and `List<int>.Sort()`. The values stay unboxed inside the arrays.

## Take-aways
1. **Boxing shows up as allocations of a primitive type.** If memory's top type is `System.Int32` or `System.Double`, look for `object`, non-generic collections, or an interface call on an unconstrained struct.
2. The remaining 4 MB is `List<int>` growing by doubling (about 2x the final size) plus the dictionary. `new List<int>(capacity)` removes most of it.
3. Boxing also hides in `string.Format("{0}", 5)`, `params object[]`, `IComparable` (non-generic), and `struct` values used through `object`-typed APIs. Interpolated strings avoid many of these in modern C#.

## Go further
Replace the dictionary with an `int[]` histogram, `CollectionsMarshal.GetValueRefOrAddDefault`, or a single pass that computes sum/max while adding.
Then write a case where the generic version is *not* faster (hint: tiny N).