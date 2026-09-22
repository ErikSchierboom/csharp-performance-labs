# L9 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Allocations:** `string` (~120 KB, LOH), `StringBuilder` chunk `char[]`s, transcoding buffers per request. **Gen2 GCs** driven by LOH allocation.

## Root cause
Buffering the whole request body as a string before parsing: a large UTF-16 copy (on the LOH) plus intermediate buffers per request, repeated for every request.

## Fix
Deserialise directly from `Request.Body` (`JsonSerializer.DeserializeAsync` or `ReadFromJsonAsync<T>()`), so the payload is read in pooled UTF-8 chunks and never becomes a string.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen2 / run | p99 latency |
|---|---|---|---|---|
| before | ≈ 324 ms | 639.61 MB | 10 | ≈ 18 ms |
| after | ≈ 361 ms | 350.05 MB | 0 | ≈ 14 ms |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Don't turn request bodies into strings** unless you must; stream them into the parser.
2. LOH-sized garbage on every request shows up as gen2 GCs and latency spikes, not as a slow function (Level 2 again).
3. Framework helpers (`ReadFromJsonAsync`, model binding) already stream; hand-written 'read the body then parse' code is where this creeps in.
4. Set a request-size limit (`MaxRequestBodySize`) regardless: unbounded bodies are also a denial-of-service vector.

## Go further
Add a 10 MB body and compare peak memory for the two versions. Then use `PipeReader` (`ctx.Request.BodyReader`) with `Utf8JsonReader` for a fully streaming parse of a huge array.

## Further reading
- [ASP.NET Core Best Practices (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)
- [Memory management and patterns in ASP.NET Core (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)
- [Conroy, Performance Improvements in ASP.NET Core 8](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)
