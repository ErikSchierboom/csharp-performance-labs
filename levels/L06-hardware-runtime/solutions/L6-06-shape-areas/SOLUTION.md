# L6-06 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Disassembly:** `call [rax+…]` (indirect call) per element in the slow loop; inlined multiplies in the fix.
- **`perf stat`:** many `branch-misses` on the indirect branch (slow).

## Root cause
Interface dispatch over a heterogeneous sequence in random order: an unpredictable indirect call per element that also blocks inlining.

## Fix
Store the data grouped by concrete type (three arrays) and loop over each. Every call disappears (inlined field arithmetic). Other fixes: sort/group the interface array by type; use a `switch` on a tag field; generics with struct constraints (`where T : struct, IShape`) so the JIT specialises per type.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 205 ms | 0.00 MB |
| after | ≈ 61 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Virtual/interface calls are cheap when predictable and costly when not**; the cost is branch misprediction plus lost inlining.
2. `sealed` helps only when the JIT can see the exact type; here it can't (the static type is the interface).
3. Grouping by type (or a struct-generic approach) is a standard data-oriented fix for polymorphic hot loops.
4. Dynamic PGO (on by default since .NET 8) devirtualises the *dominant* type per call site; it can't rescue an even mix.

## Go further
Keep the interface array but sort it by type once; is that enough? Then try the generic struct-constraint approach.

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- [Akinshin, Pro .NET Benchmarking](https://www.apress.com/us/book/9781484249406)
- [dotnet/runtime: DynamicPgo.md (guarded devirtualization)](https://github.com/dotnet/runtime/blob/main/docs/design/features/DynamicPgo.md)
