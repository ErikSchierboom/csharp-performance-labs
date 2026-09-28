# L06-02 - Solution

## What the profile shows
- **Sampling:** the loop body; nothing else.
- **`perf stat`:** far more cache/TLB misses and stalled cycles for the reference array.

## Root cause
Array of class instances = array of pointers. When the objects aren't laid out in the order you visit them, each access is a cache miss (pointer chasing) even though the code is a simple loop.

## Fix
Make `Particle` a `struct` so the array holds the data itself, contiguous in memory. (Iterate with `ref readonly` over a span to avoid copying.) Trade-off: structs copy on assignment/pass, so keep them small or pass by `in`/`ref`.

## Take-aways
1. **Reference-type arrays are arrays of pointers**; locality depends on where the allocator (and later, GC compaction) put the objects.
2. Struct arrays / structure-of-arrays are the standard fix for hot numeric loops (games, simulation, analytics).
3. The GC compacts in address order, so freshly allocated data is often *better* laid out than this exercise's shuffled data, but long-lived, mutated collections drift.
4. Don't convert everything to structs: large structs copy (L06-05) and mutable structs bite. Do it where the profile and the counters say so.

## Extra credit
Remove the shuffle in the *slow* version's `Create`. How close does the class array get, and what does that tell you about the allocator?

## Go further
Convert to structure-of-arrays (separate `int[] X, Y, ...`) and vectorise the sum. Which layout wins if the loop touches only `X`?
