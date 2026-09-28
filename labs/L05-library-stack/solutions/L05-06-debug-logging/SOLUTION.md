# L05-06 - Solution

## What the profile shows

- **Allocations:** a `string` per call (the interpolated message), plus the boxed `long`/format work; nothing is written anywhere.

## Root cause
Interpolated strings passed to `ILogger` are formatted eagerly, at the call site, regardless of the configured level. Disabled logging still pays for building the message.

## Fix
Use `[LoggerMessage]` source-generated methods (or `LoggerMessage.Define`): strongly typed arguments, no boxing, and formatting only if the level is enabled. Message templates (`"…{OrderId}…", i`) are the next best: they defer formatting but still box value-type arguments.

## Take-aways
1. **Never use string interpolation for log messages.** It defeats level filtering and structured logging (analyzer CA2254 warns about it).
2. Disabled != free: the *arguments* are still evaluated. Guard expensive arguments with `IsEnabled`.
3. `[LoggerMessage]` is the zero-allocation, structured, fast path; use it in hot code.
4. Structured templates keep the properties (`OrderId`, `Customer`) queryable in your log store.

## Extra credit
Add an `if (Log.IsEnabled(LogLevel.Debug))` guard to the *interpolated* version. Does it remove the cost? What is the downside compared with `[LoggerMessage]`?

## Go further
Compare three versions: interpolated, template with args, source-generated. Then enable Debug and compare again. Which is best when enabled?
