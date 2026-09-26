# L09-02 - Solution

## What the profile shows
- **Allocations:** `Regex` internals (parser, code), `Match`/`Group`, interpolated `string`, header strings, per request.

## Root cause
Loop-invariant work done per request (a `new Regex` in the pipeline) plus eagerly formatted log messages for a disabled level.

## Fix
A `static readonly Regex` (compiled or `[GeneratedRegex]`) and a `[LoggerMessage]` source-generated log method. Middleware runs on **every** request, so its per-request cost is multiplied by your entire traffic.

## Take-aways
1. **Middleware is on every request's critical path.** A microsecond there is a CPU-second per million requests.
2. Everything you learned about allocation (Levels 1, 2, 5) applies unchanged; the request rate is what multiplies it.
3. Hoist anything that doesn't depend on the request to a static or singleton.
4. Measure per request before and after; budgets on bytes/request catch this class of regression.

## Extra credit
Move the log call *after* `next()` (to include status code). What does that do to the p99 for slow requests, and why?

## Go further
Use `[GeneratedRegex]` instead of `RegexOptions.Compiled` and compare. Then replace the regex entirely with `PathString`/span parsing.
