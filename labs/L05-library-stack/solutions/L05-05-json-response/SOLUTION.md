# L05-05 - Solution

## What the profile shows
- **Sampling:** most of the time is metadata construction: `JsonSerializerOptions` / `JsonTypeInfo` creation, `JsonStringEnumConverter` factory work, property and naming-policy setup, repeated on all 100 requests. The actual writing is a small slice.
- **Allocation:** a new set of metadata objects per request, plus a UTF-16 `string` and a UTF-8 `byte[]` the size of each response.

## Root cause
Two separate misuses of the library, each fixed on its own line:

1. **Options per call.** `JsonSerializerOptions` holds the metadata cache. A fresh instance per request rebuilds it every time. .NET 7+ softens this by sharing the cache between *equal* options, but a `new JsonStringEnumConverter()` is a different instance each time, so no two options are ever equal and nothing is reused.
2. **Going through a string.** `Serialize` > UTF-16 `string` > `Encoding.UTF8.GetBytes` > `Write` copies the body twice before it reaches the socket. The serializer can write UTF-8 straight into the stream using pooled buffers.

## Fix
Declare the API's JSON conventions once, in a source-generated `JsonSerializerContext` (`camelCase`, `UseStringEnumConverter`, `WhenWritingNull`), and call `JsonSerializer.Serialize(responseBody, dto, ApiJson.Default.OrdersPage)`. A `static readonly JsonSerializerOptions` plus `Serialize(stream, ...)` passes too. Source generation also moves the metadata work to compile time and makes the code trim- and AOT-safe.

In ASP.NET Core you get this from `Results.Json(dto, ApiJson.Default.OrdersPage)` or `ConfigureHttpJsonOptions`: configure the options once at startup, and the framework writes straight to the response body.

## Take-aways
1. **`JsonSerializerOptions` is a cache, not a settings bag.** Create it once and reuse it (analyzer CA1869 flags the per-call pattern).
2. "Equal options share a cache" breaks as soon as a converter is `new`-ed inline.
3. **Serialise once, to the final encoding, directly into the destination.** Don't go via `string` when the wire format is UTF-8.
4. Source generation (`[JsonSerializable]`) gives the conventions a single, compile-time-checked home.

## Extra credit
Check the split in "Root cause" yourself: fix each problem on its own and measure it. Then delete the `Converters = { ... }` line, keep everything else as it was, and measure again. Why does that one line matter so much?

## Go further
Use `JsonSerializer.SerializeAsync` against a real `HttpListener` response and compare. Then return an `IAsyncEnumerable<Order>` for an unpaged export and watch memory stay flat as the row count grows.
