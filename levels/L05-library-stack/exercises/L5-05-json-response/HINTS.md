# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type: `string` (many, large) and `char[]`/`List<string>`, before the final `byte[]`.
</details>

<details><summary>Hint 2: where?</summary>

Count the *copies* of the data between the `Order` objects and the final `byte[]`. Each `Serialize`, `Join` and `GetBytes` produces a new one.
</details>

<details><summary>Hint 3: why?</summary>

JSON can be written straight to UTF-8 bytes (`SerializeToUtf8Bytes`, or a `Utf8JsonWriter` over a pooled buffer, or directly to the response stream), skipping the UTF-16 strings entirely. A *source-generated* `JsonSerializerContext` also avoids reflection-based metadata setup (important for startup and trimming/AOT).
</details>
