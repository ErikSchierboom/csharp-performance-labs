# L5-06 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory:** a `string` per call (the interpolated message), plus the boxed `long`/format work; nothing is written anywhere.

## Root cause
Interpolated strings passed to `ILogger` are formatted eagerly, at the call site, regardless of the configured level. Disabled logging still pays for building the message.

## Fix
Use `[LoggerMessage]` source-generated methods (or `LoggerMessage.Define`): strongly typed arguments, no boxing, and formatting only if the level is enabled. Message templates (`"…{OrderId}…", i`) are the next best: they defer formatting but still box value-type arguments.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 15 ms | 41.20 MB |
| after | ≈ 4.4 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Never use string interpolation for log messages.** It defeats level filtering and structured logging (analyzer CA2254 warns about it).
2. Disabled ≠ free: the *arguments* are still evaluated. Guard expensive arguments with `IsEnabled`.
3. `[LoggerMessage]` is the zero-allocation, structured, fast path; use it in hot code.
4. Structured templates keep the properties (`OrderId`, `Customer`) queryable in your log store.

## Go further
Compare three versions: interpolated, template with args, source-generated. Then enable Debug and compare again. Which is best when enabled?

## Further reading
- Microsoft Learn: High-performance logging with LoggerMessage / compile-time logging source generation *(title only)*
- Microsoft Learn: Analyzer CA2254: Template should be a static expression *(title only)*
