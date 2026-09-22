# L6-04 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling/timeline:** all threads on-CPU, 100% in the loop; no waiting.
- **`perf stat`/`perf c2c`:** heavy cross-core cache-line transfers on one line.

## Root cause
The eight counters live in adjacent array elements, so they occupy one 64-byte cache line. Independent atomic increments from different cores still fight over ownership of that line (MESI coherence traffic).

## Fix
Pad so each counter sits on its own cache line (stride of 8 `long`s). Better still: accumulate in a local variable and write the shared slot once (no sharing at all).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 552 ms | 0.00 MB |
| after | ≈ 94 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **False sharing: independent data, shared cache line.** Looks like parallelism, behaves like contention.
2. No profiler shows it as a wait; look at *scaling* (more threads, no speed-up) and at hardware counters.
3. Padding costs memory; use it for hot, per-thread data. Thread-local accumulation merged at the end is simpler and faster.
4. The size of the effect depends on the CPU topology (cores sharing a cache complex vs across them): measure on the target hardware.

## Go further
Rewrite with a local `long` per thread merged at the end, and compare with the padded version. Try stride 4 and 16: where does the effect vanish, and what does that say about your CPU's line size?

## Further reading
- [Drepper, What Every Programmer Should Know About Memory (PDF)](https://www.akkadia.org/drepper/cpumemory.pdf)
- [Bakhvalov, Performance Analysis and Tuning on Modern CPUs (free PDF)](https://book.easyperf.net/perf_book)
- [Akinshin, Pro .NET Benchmarking](https://www.apress.com/us/book/9781484249406)
