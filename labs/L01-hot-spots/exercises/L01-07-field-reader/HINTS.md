# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling first (which frames?), then allocations by type.
</details>

<details><summary>Hint 2: where?</summary>

Inside the per-item loop, which calls are about *finding* the property rather than *reading* it?
</details>

<details><summary>Hint 3: why?</summary>

Reflection has two costs: looking the member up and invoking it (with boxing of the result). Neither depends on the item, so do the lookup once. Then convert to a typed delegate (or a `switch`/source generator) so the per-item cost is a plain call.
</details>
