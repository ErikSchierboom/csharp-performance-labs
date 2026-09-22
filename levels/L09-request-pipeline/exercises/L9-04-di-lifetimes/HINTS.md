# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations: the biggest allocations per request are `Dictionary<int,int>` internals. dotTrace: who calls the `PriceCalculator` constructor, and how often?
</details>

<details><summary>Hint 2: where?</summary>

Count constructions of `PriceCalculator` per request. Where is the container created, and what lifetime is the service registered with?
</details>

<details><summary>Hint 3: why?</summary>

A `ServiceProvider` built inside a request builds (and disposes) everything on every request; a *Transient* registration constructs a new instance per resolve. Expensive-to-build, read-only reference data belongs in a **singleton** registered once in the app's container.
</details>
