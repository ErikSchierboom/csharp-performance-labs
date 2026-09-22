# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Downstream calls per incoming request (`downstreamCalls` ÷ 400) and the number of requests that give up (`giveUps`).
</details>

<details><summary>Hint 2: where?</summary>

When the downstream is overloaded and failing fast, what does an immediate retry do to the number of calls in flight?
</details>

<details><summary>Hint 3: why?</summary>

Retries are extra load, sent exactly when the dependency is weakest: a **retry storm**. The cure is to prevent overload (a **bulkhead/concurrency limit**), retry sparingly with **backoff + jitter** and a **retry budget**, and fail fast (circuit breaker) rather than pile on.
</details>
