# L02-07 - Solution

## What the profile shows
- **allocations:** ~1,000,000 short `System.String` objects (60–80 bytes each) from `DefaultInterpolatedStringHandler`; the dictionary itself is a small part.
- **tracing:** `string.GetHashCode` / `Marvin` hashing and `string.Equals` are high: hashing the same texts again and again.

## Root cause
The dictionary key is a **string built per request** by interpolation (`"{tenant}:{route}:{status}"`): an allocation and a full-string hash per event, plus a second lookup. Reading the table back needs `Split` and `int.Parse` to undo the formatting: the structured data was flattened into a string and re-parsed.

## Fix
Use a struct key: `readonly record struct Key(int Tenant, int Route, int Status)`. A record struct gets value equality, a good `GetHashCode` and `IEquatable<Key>` for free, so `Dictionary` compares it without boxing and no per-event heap object exists. The read-back needs no parsing: the key already has its fields.

## Take-aways
1. **Don't flatten structured data into strings to use as keys.** You pay to allocate, hash, compare, and later parse them.
2. Value-type keys need `IEquatable<T>`; otherwise `EqualityComparer<T>.Default` falls back to the object-based comparison and **boxes on each call.** `record struct` and `ValueTuple` implement it for you.
3. The 1.9 MB left is the dictionary's own arrays as it grows; `new Dictionary<Key,int>(capacity)` trims it.
4. The budget gate is on *bytes*: the broken struct version was fast (29 ms) but would still fail the allocation budget. That is the point of gating both.

## Go further
Pack the three values into one `long` key. Faster or slower than the `record struct`? What would you lose (readability, extensibility)? Then use `GetAlternateLookup` (if your target framework has it) for string lookups without allocation.
