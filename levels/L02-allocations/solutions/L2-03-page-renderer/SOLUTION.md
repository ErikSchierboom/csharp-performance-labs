# L2-03 · Solution

## What the profile shows
> **About the profile section.** I have not captured Rider/dotTrace/dotMemory output for this exercise. What is described is what the code and the harness numbers imply. The harness figures (time, MB, GC counts) were measured. Where your profiler shows something different, trust the profiler and note the difference in `templates/LAB-LOG.md`.

- **Harness output:** gen0 = gen1 = gen2 ≈ 187 per run. Every collection is a full one, the signature of LOH-driven collections.
- **dotMemory:** 6,000 allocations of `System.Byte[]` at 100,000 bytes each, all in the LOH, all dead by the next iteration.
- **dotTrace (timeline or GC view):** a large share of wall time is GC and zeroing fresh memory, not your rasteriser.

## Root cause
`new byte[100_000]` is ≥ 85,000 bytes, so it is allocated on the **Large Object Heap**. The LOH is only reclaimed by gen2 collections, and a burst of LOH allocation triggers them.
Six thousand pages times 100 KB is 600 MB of LOH allocation, each array freshly zeroed by the runtime, plus one full GC roughly every 32 pages.

## Fix
Rent the buffer from `ArrayPool<byte>.Shared` and **return it in `finally`**. Two rules from the pool's contract are what the exercise really tests:
1. A rented array may be **longer** than requested (the shared pool rounds up, typically to a power of two): use `AsSpan(0, PageBytes)`, never `.Length`.
2. A rented array is **not zeroed**; it may hold the previous renter's data. The original code relied on a blank canvas, so call `Clear()` on the slice (or rent with the pool's clear-on-return option and clear at return time).
Without the `Clear()` the checksum is wrong and the harness exits with code 2. I tested exactly that.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen2 collections / run |
|---|---|---|---|
| before | ≈ 120 ms | 572 MB | 187 |
| after (`ArrayPool<byte>`) | ≈ 17 ms | 0.00 MB | 0 |

Absolute times depend on your machine and runtime version; the *ratios* and the allocation numbers should look similar.

## Take-aways
1. **When gen0, gen1 and gen2 counts are equal, suspect the LOH.** Bytes allocated alone would not have told you this: an ordinary 570 MB would be mostly gen0.
2. Pooling trades allocation for a *correctness obligation*: you now own clearing, sizing and returning. A pool bug is quieter than a slow program (data from a previous request leaking into the next one is a security problem too).
3. `ArrayPool<T>.Shared` is thread-safe and good for short-lived buffers. For very large or long-lived ones, or if you need exact length, consider a custom pool or `RecyclableMemoryStream`.
4. Return the buffer in `finally`, or leak it (the pool just allocates a replacement, so nothing crashes; you simply lose the benefit).

## Go further
Rent with `clearArray: true` on **Return** instead, and compare. Then try `stackalloc` for a size that is safe on the stack, and say why 100 KB is *not* safe there.

## Further reading
- [Large object heap (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap)
- [Sitnik, Pooling large arrays with ArrayPool](https://adamsitnik.com/Array-Pool/)
- Kokosa et al., the LOH chapter
