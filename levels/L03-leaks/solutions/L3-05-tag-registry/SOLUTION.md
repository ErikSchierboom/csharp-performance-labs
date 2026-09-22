# L3-05 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory:** +20,000 `Document`, `byte[]` (4,096), `Metadata`, `string`. Retention path: static `Dictionary<object, Metadata>` in `Tags` → entries → `Document`.

## Root cause
Using a normal `Dictionary` to hang data off objects makes the dictionary an owner of those objects: it holds every key strongly and has no idea when the key is otherwise finished.

## Fix
Use `ConditionalWeakTable<object, Metadata>`. It holds keys weakly, and the value lives exactly as long as the key. Rule: the value must not strongly reference its own key from outside the table's control, or you rebuild the leak (the table handles a value referencing its key, but a *static* reference from elsewhere would not be).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 10 ms | 81.17 MB | 80.57 MB |
| after | ≈ 7.4 ms | 81.80 MB | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **A dictionary keyed by objects you don't own is a leak waiting to happen**; ask 'who removes the entry?'.
2. `ConditionalWeakTable` ties the entry's life to the key's; `WeakReference<T>` inside a normal collection still needs someone to remove dead entries.
3. It's the mechanism behind 'attached properties' and many caches keyed on objects. It compares keys by *reference*, always.
4. `Reset()` here is just so that runs don't pile up; production code has no such button.

## Go further
Replace the table with a `Dictionary<int, WeakReference<Document>>` and add cleanup. Count how much code you needed compared with `ConditionalWeakTable`.

## Further reading
- [Ferrandez, Buggy Bits debugging labs](https://github.com/TessFerrandez/BuggyBits)
- [Debug a memory leak tutorial (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak)
- [dotMemory Unit: assert on retained objects in a test](https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html)
- Microsoft Learn: ConditionalWeakTable<TKey,TValue> and WeakReference<T> API docs *(title only)*
