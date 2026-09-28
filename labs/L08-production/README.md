# Lab 8: production (beyond the IDE)

No source code, no IDE, no debugger: just a running process and the CLI tools. Closer to what an actual production incident hands you.

**Skills:** Diagnose a compiled process you have no source for, with the CLI tools only.

**Mastery checkpoint:** Given only a running process, produce a defensible diagnosis.

No `exercises/`/`solutions/` split here: each lab is a compiled "mystery service" with no source in front of you, plus a `QUESTIONS.md` to fill in before checking `ANSWERS.md`.

## Setup (once)
```powershell
./labs/L08-production/build-all.ps1     # publishes each lab's mystery service to <lab>/bin
```
Requires [PowerShell 7+](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell) (`pwsh`), runs the same on Windows, Linux and macOS.

## Working a lab
1. Start the lab's service in one terminal: `./labs/L08-production/<lab>/run.ps1 <args>` (each lab's README gives the exact args; it prints its own pid).
2. Point the CLI tools at that pid from a second terminal: `dotnet-counters`, `dotnet-trace`, `dotnet-gcdump`, `dotnet-dump`, per the lab.
3. Fill in `<lab>/QUESTIONS.md` **before** opening `<lab>/ANSWERS.md`.

Labs: [L08-01-triage](L08-01-triage/README.md) · [L08-02-flame-graph](L08-02-flame-graph/README.md) · [L08-03-who-holds-memory](L08-03-who-holds-memory/README.md) · [L08-04-container-limits](L08-04-container-limits/README.md) · [L08-05-perf-gate](L08-05-perf-gate/README.md)

**Further reading:** [Lab 8 reading list](../../docs/READING-LIST.md#lab-8-beyond-the-ide)

## Counter names: .NET ≤ 8 vs. .NET 9+
Starting in .NET 9, `dotnet-counters monitor -p <pid> System.Runtime` reports the new `System.Runtime` **Meter** (`dotnet.*` names) instead of the old EventCounters used by earlier runtimes and by most existing docs/blog posts. This repo targets .NET 10, so you'll see the right-hand column. The counters these labs actually use:

| Old EventCounter name | New (.NET 9+) meter name | Notes |
|---|---|---|
| `threadpool-queue-length` | `dotnet.thread_pool.queue.length` | Same meaning: work items currently queued. |
| `threadpool-thread-count` | `dotnet.thread_pool.thread.count` | Same meaning: worker threads that currently exist. |
| `gc-heap-size` | `dotnet.gc.last_collection.heap.size` | Now tagged by generation (`gen0`/`gen1`/`gen2`/`loh`/`poh`) and only updates once per collection, not on a timer. |
| `gen-0-gc-count`, `gen-1-gc-count`, `gen-2-gc-count` | `dotnet.gc.collections` | One metric now, not three: the `gc.heap.generation` tag (`gen0`/`gen1`/`gen2`) tells them apart. |
| `alloc-rate` | `dotnet.gc.heap.total_allocated` | Old was bytes/sec; new is a cumulative total since process start, diff two readings to get a rate, same idea `Lab.cs` uses internally. |
| `time-in-gc` | `dotnet.gc.pause.time` | Old was a % over the sample window; new is cumulative seconds paused since start, diff and divide by wall time for a %. |
| `monitor-lock-contention-count` | `dotnet.monitor.lock_contentions` | Same meaning: cumulative contended-lock count. |
| `exception-count` | `dotnet.exceptions` | Same meaning, now tagged by exception type (`error.type`). |

Want the old names anyway? `dotnet-counters monitor -p <pid> --counters EventCounters\System.Runtime` still works. Source: [.NET runtime metrics (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/built-in-metrics-runtime).

