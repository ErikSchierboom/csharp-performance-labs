# L6-02 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** the loop body; nothing else.
- **`perf stat`:** far more cache/TLB misses and stalled cycles for the reference array.

## Root cause
Array of class instances = array of pointers. When the objects aren't laid out in the order you visit them, each access is a cache miss (pointer chasing) even though the code is a simple loop.

## Fix
Make `Particle` a `struct` so the array holds the data itself, contiguous in memory. (Iterate with `ref readonly` over a span to avoid copying.) Trade-off: structs copy on assignment/pass, so keep them small or pass by `in`/`ref`.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 18 ms | 0.00 MB |
| after | ≈ 2.8 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Reference-type arrays are arrays of pointers**; locality depends on where the allocator (and later, GC compaction) put the objects.
2. Struct arrays / structure-of-arrays are the standard fix for hot numeric loops (games, simulation, analytics).
3. The GC compacts in address order, so freshly allocated data is often *better* laid out than this exercise's shuffled data, but long-lived, mutated collections drift.
4. Don't convert everything to structs: large structs copy (L6-05) and mutable structs bite. Do it where the profile and the counters say so.

## Go further
Convert to structure-of-arrays (separate `int[] X, Y, ...`) and vectorise the sum. Which layout wins if the loop touches only `X`?

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- [Akinshin, Pro .NET Benchmarking](https://www.apress.com/us/book/9781484249406)
