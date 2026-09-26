# L09-05 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Allocations:** `string` (~120 KB, LOH), `StringBuilder` chunk `char[]`s, transcoding buffers per request. **Gen2 GCs** driven by LOH allocation.

## Root cause
Buffering the whole request body as a string before parsing: a large UTF-16 copy (on the LOH) plus intermediate buffers per request, repeated for every request.

## Fix
Deserialise directly from `Request.Body` (`JsonSerializer.DeserializeAsync` or `ReadFromJsonAsync<T>()`), so the payload is read in pooled UTF-8 chunks and never becomes a string.

## Take-aways
1. **Don't turn request bodies into strings** unless you must; stream them into the parser.
2. LOH-sized garbage on every request shows up as gen2 GCs and latency spikes, not as a slow function (Level 2 again).
3. Framework helpers (`ReadFromJsonAsync`, model binding) already stream; hand-written 'read the body then parse' code is where this creeps in.
4. Set a request-size limit (`MaxRequestBodySize`) regardless: unbounded bodies are also a denial-of-service vector.

## Extra credit
Call `ctx.Request.EnableBuffering()` in the fix. What does that do to allocation, and when would you need it?

## Go further
Add a 10 MB body and compare peak memory for the two versions. Then use `PipeReader` (`ctx.Request.BodyReader`) with `Utf8JsonReader` for a fully streaming parse of a huge array.
