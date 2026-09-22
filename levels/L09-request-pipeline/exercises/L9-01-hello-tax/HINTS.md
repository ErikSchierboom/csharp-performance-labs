# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Measure bytes and CPU **per request** (the harness's total ÷ 4,000). Then profile allocations: which types are allocated per request, and by which layer (Kestrel, routing, MVC, formatters)?
</details>

<details><summary>Hint 2: where?</summary>

Follow one request through the pipeline: routing → controller activation → action invocation → result execution → output formatter. Which of those layers exist for a minimal API?
</details>

<details><summary>Hint 3: why?</summary>

MVC gives you model binding, filters, content negotiation and formatters, which cost allocations and CPU per request even for a trivial action. A minimal endpoint that returns text or a pre-built result skips them. That's not an argument against MVC; it tells you the *floor* to compare your real endpoints against.
</details>
