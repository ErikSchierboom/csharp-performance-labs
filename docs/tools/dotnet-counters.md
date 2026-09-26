# dotnet-counters

**Answers:** what is the process doing *overall*, right now? CPU, allocation rate, GC counts and heap size, thread pool queue, lock contention, exceptions. It's the cheapest first look: almost no overhead, and nothing to open afterwards.

## Commands
```bash
dotnet-counters ps                                     # list .NET processes
dotnet-counters monitor -n <id>                        # live view of System.Runtime, refreshed every second
dotnet-counters monitor -n <id> --showDeltas           # add a column with the change since the last refresh
dotnet-counters monitor -n <id> --counters System.Runtime,Microsoft.AspNetCore.Hosting

# record to a file instead of watching
dotnet-counters collect -n <id> --counters System.Runtime --format csv -o run.csv --duration 00:00:00:30
```
Press `q` to stop `monitor`. `--duration` is `dd:hh:mm:ss`.

## Reading it
Watch it for 10 to 20 seconds before you decide anything. A single refresh tells you little; the *trend* is the signal.

| You see | It suggests |
|---|---|
| `dotnet.gc.heap.total_allocated` climbing fast | lots of allocation: look with an allocation profiler |
| `dotnet.gc.collections` for `gen2` climbing | full collections: large objects, or memory that survives too long |
| `dotnet.gc.last_collection.heap.size` growing and never dropping | something keeps objects alive: take two [gcdumps](dotnet-gcdump.md) and compare |
| `dotnet.thread_pool.queue.length` above zero, CPU low | work is waiting for threads: blocked threads, see [dotnet-stack](dotnet-stack.md) |
| `dotnet.thread_pool.thread.count` creeping up | the pool is adding threads because the existing ones are blocked |
| `dotnet.monitor.lock_contentions` climbing | threads are fighting over a `lock` |
| `dotnet.exceptions` climbing | exceptions used as control flow, or a hidden failure loop |

Several metrics are split by a tag, such as `gc.heap.generation` (`gen0`, `gen1`, `gen2`, `loh`, `poh`). Read the rows under the metric name, not just the first one.

## Traps
- **The names changed in .NET 9.** Most blog posts use the old ones (`alloc-rate`, `gc-heap-size`, `threadpool-queue-length`). This lab runs .NET 10, so you get the `dotnet.*` names. The [Level 8 README](../../levels/L08-production/README.md#counter-names-net-8-vs-net-9) has the full mapping, and `--counters EventCounters\System.Runtime` still shows the old ones.
- **Some values are totals since the process started**, not rates (`total_allocated`, `collections`, `gc.pause.time`). Use `--showDeltas`, or subtract two readings.
- **The heap size only updates after a GC.** If nothing is collecting, the number doesn't move, even while memory grows.

## Docs
[dotnet-counters (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters) · [Built-in runtime metrics](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/built-in-metrics-runtime)
