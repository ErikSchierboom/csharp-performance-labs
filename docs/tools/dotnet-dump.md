# dotnet-dump

**Answers:** anything, after the fact. A dump is a copy of the process's memory: every object with its fields, every thread with its stack, every lock. You take it once and inspect it offline for as long as you like.

## Commands
```bash
dotnet-dump collect -n <id> -o app.dmp                 # full dump (default)
dotnet-dump analyze app.dmp                            # interactive prompt; type 'help', 'exit' to leave

# or run commands non-interactively
dotnet-dump analyze app.dmp -c "dumpheap -stat" -c "exit"
```

## Commands inside `analyze`
| Command | Shows |
|---|---|
| `dumpheap -stat` | every type on the heap: count and total size, smallest first (so the big ones are at the bottom) |
| `dumpheap -stat -min 100000` | only objects of 100 KB or more (large buffers) |
| `dumpheap -mt <MT>` | every object of one type (the `MT` column from `-stat`) |
| `dumpobj <address>` | one object's fields |
| `gcroot <address>` | the chain of references that keeps an object alive: **the** leak command |
| `clrthreads` | managed threads, and which ones are the GC and finalizer threads |
| `clrstack -all` | the managed stack of every thread |
| `pstacks` | stacks merged by call path: "40 threads are all in `Task.Wait`" at a glance |
| `syncblk` | which threads own a `lock`, and how many are waiting on it |
| `threadpool` | thread pool worker counts and queue length |
| `dumpasync` | async state machines on the heap, for "stuck" async code |
| `finalizequeue` | objects waiting for finalization |

A typical leak hunt: `dumpheap -stat` → pick the suspicious type → `dumpheap -mt <MT>` → take one address → `gcroot <address>` → read the chain from the root down to your object.

## Traps
- **Dumps are big.** A full dump of a small exercise was 176 MB. `--type Heap` is smaller and still has the whole managed heap. `--type Mini` has only threads and stacks.
- **A dump contains everything in memory**, including secrets and customer data in a real service. Treat it like a database export.
- The process is paused while the dump is written.
- Open the dump on the same OS it was taken on. A Linux dump can't be analysed with `dotnet-dump` on Windows, and vice versa.

## Docs
[dotnet-dump (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-dump) · [SOS commands](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/sos-debugging-extension)
