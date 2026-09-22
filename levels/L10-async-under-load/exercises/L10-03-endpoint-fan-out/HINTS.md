# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

There's no CPU problem. Watch **concurrency**: the `peakInflight` metric (in real life, the downstream's own dashboard or your outbound `HttpClient` metrics).
</details>

<details><summary>Hint 2: where?</summary>

Each request fans out 30-wide. How many requests are in flight at once, and what is the product?
</details>

<details><summary>Hint 3: why?</summary>

Per-request parallelism multiplies by request concurrency: 16 users × 30 calls = 480 in flight. A limit *per request* doesn't bound the total. Put the bound where the shared resource is: a semaphore (or a limited `HttpClient` handler / bulkhead) **shared by all requests**.
</details>
