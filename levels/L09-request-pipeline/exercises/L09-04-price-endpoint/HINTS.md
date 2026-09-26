# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations: per request, look at the `string`s and `Dictionary<int, decimal>` internals, and at what allocates them. dotTrace: which constructor shows up in every request, and how many times does it run per 1,500 requests?
</details>

<details><summary>Hint 2: where?</summary>

Count how often `PriceCatalog` is constructed. Who creates it? Look at how the services are registered in the `builder.Services` block.
</details>

<details><summary>Hint 3: why?</summary>

A *transient* service gets a new instance every time it's resolved, and so does everything it depends on that's transient too. `PricingService` is resolved per request, so the price list is parsed per request. Expensive-to-build, read-only reference data belongs in a **singleton**.
</details>
