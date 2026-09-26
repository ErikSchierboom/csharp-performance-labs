# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Start with a sampling profile, then dotMemory allocations by type. Look for time inside `System.Text.Json.Serialization.Metadata` (`JsonTypeInfo`, converter factories, property setup), not in the actual writing. In memory, look for `string` and `byte[]` roughly the size of each response.
</details>

<details><summary>Hint 2: where?</summary>

The serializer builds metadata for every type it sees (properties, names, converters) and caches it on the `JsonSerializerOptions` instance. How long does that options instance live? Then count how many copies of each response exist before it reaches `responseBody`.
</details>

<details><summary>Hint 3: why?</summary>

A new `JsonSerializerOptions` per call throws the metadata cache away. .NET 7+ lets a new options object reuse the cache of an *equal* one, but converters are compared by reference, so `new JsonStringEnumConverter()` makes every call's options different. Create the options once (a `static readonly` field, or a source-generated `JsonSerializerContext`). Separately, `Serialize` → `string` (UTF-16) → `GetBytes` → `Write` makes two full copies of the body: `JsonSerializer.Serialize(Stream, ...)` writes UTF-8 straight into the stream.
</details>
