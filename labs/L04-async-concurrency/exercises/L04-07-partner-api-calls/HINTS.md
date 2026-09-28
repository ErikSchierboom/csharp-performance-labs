# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Idle everything: it's neither CPU nor GC. Compare wall time with (calls × latency) and with (calls × latency ÷ concurrency).
</details>

<details><summary>Hint 2: where?</summary>

Which option or constant controls how many calls are in flight? What value does it have?
</details>

<details><summary>Hint 3: why?</summary>

Lab 4 taught you unbounded concurrency is dangerous; this is the opposite failure. Throughput = concurrency ÷ latency. Pick the limit from the *downstream's capacity* (measure the knee: L04-03), not from caution.
</details>
