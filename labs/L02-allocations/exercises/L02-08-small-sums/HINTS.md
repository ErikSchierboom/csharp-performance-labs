# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type: which type, and how many instances (roughly equal to the number of calls)?
</details>

<details><summary>Hint 2: where?</summary>

Which line makes a per-call object? Look at how `foreach` over an *interface-typed* parameter is compiled.
</details>

<details><summary>Hint 3: why?</summary>

`List<T>` has a struct enumerator that `foreach` uses without allocating when the static type is `List<T>`. Passed as `IEnumerable<T>`, the same struct is **boxed** (heap allocation) per call, and every element goes through interface dispatch. Accept a concrete type or, better, a span.
</details>
