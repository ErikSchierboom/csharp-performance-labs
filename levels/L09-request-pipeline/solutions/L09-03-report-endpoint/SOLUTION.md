# L09-03 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Sampling:** time in `FlushAsync`, `WriteAsync`, Kestrel's output pipe writer/socket send path; allocation of the per-line interpolated strings.

## Root cause
Flushing after every line turns one small response into hundreds of socket writes and continuations; per-response overhead is multiplied by the line count.

## Fix
Build the body once (`StringBuilder`, or better, write directly to `BodyWriter`) and write it in one call; let Kestrel flush at the end of the response. Keep streaming (with deliberate flushes) for genuinely long-lived or large responses.

## Take-aways
1. **Don't flush per item** unless a client benefits from receiving items early.
2. Chattiness is a latency multiplier: fixed per-call cost × number of calls.
3. Prefer `BodyWriter`/`IBufferWriter<byte>` and `Utf8` formatting for high-throughput endpoints; avoid `string` intermediates.
4. `Response.WriteAsync(string)` encodes to UTF-8 each call; a single large write encodes once.

## Extra credit
Keep the per-line writes but remove the `FlushAsync`. How much of the improvement do you get, and where does the rest come from?

## Go further
Write with `BodyWriter` (`Encoding.UTF8.GetBytes(line, writer.GetSpan(...))`) with no `string` allocations. Compare allocations.
