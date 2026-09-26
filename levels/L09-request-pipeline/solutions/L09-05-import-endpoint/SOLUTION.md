# L09-05 - Solution

## What the profile shows

- **Allocations:** per request, one ~340 KB `string` (the whole body in UTF-16, on the LOH), the `StringBuilder`/`char[]` buffers `ReadToEndAsync` grows on the way there, and UTF-8 transcoding buffers when the string is parsed. The `ImportLine` objects themselves are a small fraction.
- **GC:** ~35 gen2 collections per run: LOH allocations count toward gen2, so every request brings the next full collection closer.

## Root cause
The handler turns the request body into a string before parsing it: a 170 KB UTF-8 body becomes a 340 KB UTF-16 string on the Large Object Heap, built through intermediate buffers, then transcoded back to UTF-8 for the parser, on every request.

The model is already lean: `ImportLine` declares only `Id` and `Qty`, and `System.Text.Json` skips the descriptions, SKUs and prices without allocating anything for them. So the string copy isn't a small part of the cost, it's nearly all of it.

## Fix
Parse straight from the request stream: `await ctx.Request.ReadFromJsonAsync<ImportBatch>()` (or `JsonSerializer.DeserializeAsync<ImportBatch>(ctx.Request.Body)`). The serializer reads the body in pooled UTF-8 chunks, and the payload never exists as one big object. Binding the model as a handler parameter (`async (ImportBatch batch) => ...`) does the same.

## Take-aways
1. **Don't turn request bodies into strings** unless you really need the text; stream them into the parser.
2. **A lean model only helps if the parser sees the stream.** Skipping unused properties is free, but not after the whole body has been copied into a string first.
3. LOH-sized garbage on every request shows up as gen2 collections and latency spikes, not as one slow function (Level 2 again).
4. Set a request size limit (`MaxRequestBodySize`) regardless: an unbounded body is also a denial-of-service vector.

## Extra credit
Call `ctx.Request.EnableBuffering()` before parsing in the fix. What does it do to allocation, and when would you actually need it (for example, to log the body after a failed parse)?

## Go further
Make the batch 10,000 lines and compare peak memory for both versions. Then sum a huge batch without holding every line in memory: `JsonSerializer.DeserializeAsyncEnumerable<ImportLine>` streams a *top-level* array, so it would need a different payload shape; with this shape, a `PipeReader` (`ctx.Request.BodyReader`) and a `Utf8JsonReader` can walk `Lines` one item at a time.
