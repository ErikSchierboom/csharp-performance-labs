# L5-05 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory:** ~5,000 small `string`s + one big joined `string` (UTF-16, LOH-sized) + the `char`→`byte` copy.
- **Sampling:** time in `JsonSerializer.Serialize` entry overhead ×5,000, `string.Join`, and `Encoding.GetBytes`.

## Root cause
Building JSON by string manipulation: per-item serialisation to UTF-16 strings, a joined string, then an encoding pass, when the wire format is UTF-8 and the serializer can write it directly.

## Fix
`JsonSerializer.SerializeToUtf8Bytes(list, AppJson.Default.ListOrder)` with a source-generated context. In a web app, write to the response body (`Results.Json`/`WriteAsJsonAsync`) so even the byte array disappears.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 5.2 ms | 5.18 MB |
| after | ≈ 2.6 ms | 0.49 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Serialise once, to the final encoding, directly into the destination.**
2. Source generation (`[JsonSerializable]`) removes runtime reflection: faster startup, AOT/trim-friendly, and predictable allocation.
3. Don't hand-build JSON with strings; correctness (escaping) and performance both suffer.
4. For big payloads, stream: `JsonSerializer.SerializeAsync` to the response, or `IAsyncEnumerable` for lists.

## Go further
Write the array with a `Utf8JsonWriter` into an `ArrayBufferWriter<byte>` and compare allocations. Then serialise directly to a `MemoryStream`/`Stream` and compare.

## Further reading
- Microsoft Learn: How to use source generation in System.Text.Json *(title only)*
- [Toub, Performance Improvements in .NET 10 (JSON section)](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/)
