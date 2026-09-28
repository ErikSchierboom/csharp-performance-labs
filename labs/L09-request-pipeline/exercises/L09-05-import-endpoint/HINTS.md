# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type, and the **gen2 count** in the harness output: `string`, `char[]` and `StringBuilder` chunks of a size that hints at the Large Object Heap.
</details>

<details><summary>Hint 2: where?</summary>

Follow the bytes: request stream → ? → `Batch`. Count how many full copies of the payload exist at once and in which encoding.
</details>

<details><summary>Hint 3: why?</summary>

`ReadToEndAsync` builds a **UTF-16 string** (2 bytes per char: ~340 KB here, far over the 85,000-byte LOH threshold) through growing intermediate buffers, and `Deserialize(string)` then transcodes it back to UTF-8. That's L02-03 (LOH churn) on the request path. The properties the model doesn't declare are skipped without allocating, so the string is nearly all the garbage. `ReadFromJsonAsync` / `DeserializeAsync` parse UTF-8 from the stream directly.
</details>
