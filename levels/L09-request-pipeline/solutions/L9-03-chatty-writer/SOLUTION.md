# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Sampling:** time in `FlushAsync`, `WriteAsync`, Kestrel's output pipe writer/socket send path; allocation of the per-line interpolated strings.

## Root cause
Flushing after every line turns one small response into hundreds of socket writes and continuations; per-response overhead is multiplied by the line count.

## Fix
Build the body once (`StringBuilder`, or better, write directly to `BodyWriter`) and write it in one call; let Kestrel flush at the end of the response. Keep streaming (with deliberate flushes) for genuinely long-lived or large responses.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | p99 latency |
|---|---|---|---|
| before | ≈ 105 ms | 88.73 MB | ≈ 5 ms |
| after | ≈ 37 ms | 94.76 MB | ≈ 2 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Don't flush per item** unless a client benefits from receiving items early.
2. Chattiness is a latency multiplier: fixed per-call cost × number of calls.
3. Prefer `BodyWriter`/`IBufferWriter<byte>` and `Utf8` formatting for high-throughput endpoints; avoid `string` intermediates.
4. `Response.WriteAsync(string)` encodes to UTF-8 each call; a single large write encodes once.

## Go further
Write with `BodyWriter` (`Encoding.UTF8.GetBytes(line, writer.GetSpan(...))`) with no `string` allocations. Compare allocations.

## Further reading
- [ASP.NET Core Best Practices (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [Memory management and patterns in ASP.NET Core (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)
- [Conroy, Performance Improvements in ASP.NET Core 8](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)
