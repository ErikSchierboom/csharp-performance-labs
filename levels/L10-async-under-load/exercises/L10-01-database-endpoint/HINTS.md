# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

`dotnet-counters`: thread-pool queue length and thread count while the load runs (see `levels/L08-production/README.md` for the new counter names). dotTrace timeline: pool threads *Waiting*.
</details>

<details><summary>Hint 2: where?</summary>

Which call in the handler blocks the thread while the 'database' works?
</details>

<details><summary>Hint 3: why?</summary>

This is L04-01 inside a real pipeline. Blocking a pool thread per in-flight request means 200 concurrent requests need 200 threads, but the pool grows slowly; requests queue for threads while the threads wait for the delay's continuation. Make the handler async end to end and let the framework await it.
</details>
