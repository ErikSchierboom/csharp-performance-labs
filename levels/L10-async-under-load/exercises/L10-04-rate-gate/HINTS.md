# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

`dotnet-counters`: pool is *healthy* (async waits don't hold threads); latency is from **queueing at the semaphore**. Count outbound calls per second: it equals the request rate.
</details>

<details><summary>Hint 2: where?</summary>

The lock is a serialisation point for **every** request, including ones that ask for the same currency the previous request just fetched.
</details>

<details><summary>Hint 3: why?</summary>

A single global async lock turns a cacheable lookup into a serial pipeline: throughput = 1 ÷ (call time). The value is shared and slow-changing: fetch it **once per key** (with a shared `Lazy<Task>`), let everyone await the same task, and expire/refresh it deliberately.
</details>
