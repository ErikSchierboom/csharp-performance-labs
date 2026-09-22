# L5-07 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Profile:** time in file open/close and write syscalls, not in formatting.
- **`strace -c`:** ~20,000 × `openat`/`write`/`close` (and friends) vs a handful of large `write`s.

## Root cause
Per-call open/write/close. System-call and file-system overhead dominate small writes; the cost scales with the *number* of calls, not the bytes.

## Fix
Keep one `StreamWriter` with a large buffer (64 KB) open for the run and write to it; flush on a timer or at shutdown as the durability requirements dictate. Same bytes on disk (use `new UTF8Encoding(false)`: `Encoding.UTF8` writes a 3-byte BOM at the start of a new file, which `AppendAllText` doesn't).

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 43 ms | 3.51 MB |
| after | ≈ 1.5 ms | 1.69 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Batch small I/O.** Per-call syscalls, not bytes, are the cost.
2. Buffering trades durability for speed: decide explicitly how much you can afford to lose on a crash (flush interval, `AutoFlush`, `Flush(true)`).
3. Async I/O doesn't fix this (it's still one syscall per call), but it stops threads blocking; in servers, prefer a background writer fed by a `Channel`.
4. The effect depends on the file system (tmpfs vs SSD vs network), so measure where it will run.

## Go further
Write via a `Channel<string>` and a single background consumer with a buffered writer. What happens to durability on crash? What about ordering?

## Further reading
- Microsoft Learn: File I/O and StreamWriter API docs *(title only)*
- [Gregg, Systems Performance: file systems and I/O chapters](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)
