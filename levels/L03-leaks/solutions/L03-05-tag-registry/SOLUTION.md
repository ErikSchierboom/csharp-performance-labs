# L03-05 - Solution

## What the profile shows
- **snapshots:** +20,000 `Document`, `byte[]` (4,096), `Metadata`, `string`. Retention path: static `Dictionary<object, Metadata>` in `Tags` → entries → `Document`.

## Root cause
Using a normal `Dictionary` to hang data off objects makes the dictionary an owner of those objects: it holds every key strongly and has no idea when the key is otherwise finished.

## Fix
Use `ConditionalWeakTable<object, Metadata>`. It holds keys weakly, and the value lives exactly as long as the key. Rule: the value must not strongly reference its own key from outside the table's control, or you rebuild the leak (the table handles a value referencing its key, but a *static* reference from elsewhere would not be).

## Take-aways
1. **A dictionary keyed by objects you don't own is a leak waiting to happen**; ask 'who removes the entry?'.
2. `ConditionalWeakTable` ties the entry's life to the key's; `WeakReference<T>` inside a normal collection still needs someone to remove dead entries.
3. It's the mechanism behind 'attached properties' and many caches keyed on objects. It compares keys by *reference*, always.
4. `Reset()` here is just so that runs don't pile up; production code has no such button.

## Extra credit
What happens if `Metadata` holds a strong reference back to its `Document` in the `ConditionalWeakTable` version? Try it and look at the kept figure (there is a documented guarantee here).

## Go further
Replace the table with a `Dictionary<int, WeakReference<Document>>` and add cleanup. Count how much code you needed compared with `ConditionalWeakTable`.
