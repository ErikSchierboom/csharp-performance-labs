# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The harness's kept-after-GC line; dotMemory snapshot compare: `Product` and `string` growth, dominated by the DbContext's change tracker (`StateManager`).
</details>

<details><summary>Hint 2: where?</summary>

What object owns the retained `Product`s, and why does it never let go?
</details>

<details><summary>Hint 3: why?</summary>

A tracking `DbContext` keeps a reference (and a snapshot) for every entity it loads, for its whole lifetime. A context is designed to be a **unit of work**: short-lived. A long-lived one is a slow leak and a concurrency bug (`DbContext` isn't thread-safe).
</details>
