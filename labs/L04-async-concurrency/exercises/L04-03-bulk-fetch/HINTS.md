# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

There's no CPU story here; look at *concurrency*. What is the peak number of in-flight calls, and how does per-call latency change with it? (The harness reports `peakInflight`.)
</details>

<details><summary>Hint 2: where?</summary>

`Task.WhenAll(items.Select(...))` starts every call before any has finished. Where would you put a *limit*?
</details>

<details><summary>Hint 3: why?</summary>

Concurrency has a sweet spot: more parallelism raises throughput until the downstream saturates, then latency (and failures) climb faster than throughput. Which API runs an async body over a sequence with a bounded degree of parallelism?
</details>
