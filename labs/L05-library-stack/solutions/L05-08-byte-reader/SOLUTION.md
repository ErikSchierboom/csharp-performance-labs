# L05-08 - Solution

## What the profile shows
- **`strace -c`:** ~400,000 `read` syscalls.
- **Sampling**: `RandomAccess.ReadAtOffset`/`read` dominate.

## Root cause
Buffering disabled: one system call per byte.

## Fix
Use the default buffered `FileStream` (or `BufferedStream`, or read blocks into a `byte[]`/`Span<byte>` or `File.ReadAllBytes`). Same result, ~100 system calls.

## Take-aways
1. **System calls, not bytes, are expensive.** Batch I/O (also L05-07).
2. Defaults are usually right; someone set `bufferSize: 0` for a reason that may no longer apply: ask why.
3. For sequential processing, read blocks and process spans; `ReadByte` in a loop is a smell either way.

## Extra credit
Try `FileOptions.SequentialScan` and `RandomAccess.Read` with a pooled buffer.

## Go further
Read in 64 KB blocks into a `byte[]` and sum with a span loop. Then vectorise the sum with `Vector<T>`.
