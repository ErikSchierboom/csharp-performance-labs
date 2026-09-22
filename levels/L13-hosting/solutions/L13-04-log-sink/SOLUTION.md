# L13 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Timeline:** request threads blocked on the logger lock; **strace:** thousands of open/write/close.

## Root cause
Synchronous, per-line file I/O under a global lock in the logging path: request latency includes disk latency and lock queueing.

## Fix
An asynchronous provider: `Log` enqueues to a bounded channel, one background thread writes to a single buffered `StreamWriter`. Flush on shutdown, drop and count when the queue is full. (In real apps use a proven async sink: Serilog's async wrapper, OpenTelemetry exporters, or the console/JSON logger to stdout, collected by the platform.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 104 ms | 11.13 MB | ≈ 6 ms |
| after | ≈ 12 ms | 9.74 MB | ≈ 0 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Telemetry must not add latency or fail requests.** Logging, metrics and tracing are on the critical path unless you decouple them.
2. Bounded queue + drop-and-count beats blocking when the sink is slow.
3. Log less, and structure it: four lines per request is expensive at scale even when async.
4. Measure the observability tax (with and without) as part of your performance budget.

## Go further
Sample the logging (1 in 10 requests) and compare p99. What information do you lose?

## Further reading
- Kestrel web server in ASP.NET Core (Microsoft Learn) *(title only)*
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [ASP.NET Core built-in metrics](https://learn.microsoft.com/en-us/aspnet/core/metrics/built-in?view=aspnetcore-10.0)
