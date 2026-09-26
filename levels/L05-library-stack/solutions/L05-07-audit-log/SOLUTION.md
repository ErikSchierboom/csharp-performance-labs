# L05-07 - Solution

## What the profile shows

- **Sampling:** time in file open/close and write syscalls, not in formatting.
- **`strace -c`:** ~20,000 x `openat`/`write`/`close` (and friends) vs a handful of large `write`s.

## Root cause
Per-call open/write/close. System-call and file-system overhead dominate small writes; the cost scales with the *number* of calls, not the bytes.

## Fix
Keep one `StreamWriter` with a large buffer (64 KB) open for the run and write to it; flush on a timer or at shutdown as the durability requirements dictate. Same bytes on disk (use `new UTF8Encoding(false)`: `Encoding.UTF8` writes a 3-byte BOM at the start of a new file, which `AppendAllText` doesn't).

## Take-aways
1. **Batch small I/O.** Per-call syscalls, not bytes, are the cost.
2. Buffering trades durability for speed: decide explicitly how much you can afford to lose on a crash (flush interval, `AutoFlush`, `Flush(true)`).
3. Async I/O doesn't fix this (it's still one syscall per call), but it stops threads blocking; in servers, prefer a background writer fed by a `Channel`.
4. The effect depends on the file system (tmpfs vs SSD vs network), so measure where it will run.

## Extra credit
Set `AutoFlush = true` on the `StreamWriter`. How much of the win survives?

## Go further
Write via a `Channel<string>` and a single background consumer with a buffered writer. What happens to durability on crash? What about ordering?
