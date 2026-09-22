# L6-05 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** time in `Run` and the copy prologue; **disassembly:** a 512-byte block copy before each `Score()` call in the slow version, none in the fix.

## Root cause
An instance method on a *mutable* struct called through a `readonly` field forces the compiler to make a defensive copy of the whole 512-byte struct on every call.

## Fix
Make the struct `readonly` (the compiler then verifies immutability and elides the copies). Or mark just the method `readonly`. Or pass the struct by `in`/`ref readonly` to methods that take it. (The constructor here uses `Unsafe.AsRef` only to fill an `InlineArray` inside a readonly struct; a plain struct with fields wouldn't need it.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 103 ms | 0.00 MB |
| after | ≈ 33 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Big structs make hidden copies**: passing by value, `foreach` over struct collections, and calling methods through `readonly` fields all copy.
2. `readonly struct` is nearly free correctness *and* speed: default to it for value types.
3. Analyzers/IDE hints flag defensive copies (IDE0250, 'struct can be made readonly'); the disassembly proves them.
4. Rule of thumb: structs up to ~16 bytes copy cheaply; beyond that, think about `in`/`ref` or make it a class. Below ~128 bytes the effect is small (I measured 1.2× at 128 B and 3.5× at 512 B).

## Go further
Keep the mutable struct but mark only `Score()` as `readonly`. Same speed-up? Then pass the struct by `in` to a static method and compare.

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- [Akinshin, Pro .NET Benchmarking](https://www.apress.com/us/book/9781484249406)
- Microsoft Learn: Write safe and efficient C# code (readonly struct, in, ref readonly) *(title only)*
