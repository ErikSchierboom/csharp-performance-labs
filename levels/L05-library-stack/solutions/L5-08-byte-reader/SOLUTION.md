# L5-08 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **`strace -c`:** ~400,000 `read` syscalls (slow) vs ~100 (fix). Sampling: `RandomAccess.ReadAtOffset`/`read` dominate.

## Root cause
Buffering disabled: one system call per byte.

## Fix
Use the default buffered `FileStream` (or `BufferedStream`, or read blocks into a `byte[]`/`Span<byte>` or `File.ReadAllBytes`). Same result, ~100 system calls.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 63 ms | 0.00 MB |
| after | ≈ 0.7 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **System calls, not bytes, are expensive.** Batch I/O (also L5-07).
2. Defaults are usually right; someone set `bufferSize: 0` for a reason that may no longer apply: ask why.
3. For sequential processing, read blocks and process spans; `ReadByte` in a loop is a smell either way.

## Go further
Read in 64 KB blocks into a `byte[]` and sum with a span loop. Then vectorise the sum with `Vector<T>`.

## Further reading
- Microsoft Learn: FileStream, BufferedStream API docs *(title only)*
- [Gregg, Systems Performance (file systems & syscalls)](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)
