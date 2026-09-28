# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Pool metrics (connections in use vs max), or a counter of waiters on the pool. Latency splits into *waiting for a connection* and *using it*.
</details>

<details><summary>Hint 2: where?</summary>

For how long does each request hold a connection, and what is it doing during most of that time?
</details>

<details><summary>Hint 3: why?</summary>

Little's law: pool throughput = size ÷ hold time. With 8 connections held ~31 ms each, the ceiling is ~258 req/s. Hold a connection only for the work that needs it; do slow non-database work before renting or after releasing.
</details>
