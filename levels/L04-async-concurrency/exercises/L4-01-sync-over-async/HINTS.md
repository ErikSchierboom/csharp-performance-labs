# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling shows an idle process, so it will mislead you. Use dotTrace's **timeline**: look at thread states (especially *Waiting*) and the thread count over time. Also watch `threadpool-thread-count` and `threadpool-queue-length` in `dotnet-counters`.
</details>

<details><summary>Hint 2: where?</summary>

Most pool threads are sitting in a wait, inside `Handle`. Find the exact call that makes them wait, and ask what *it* is waiting for.
</details>

<details><summary>Hint 3: why?</summary>

`.Result` blocks the current thread until the task completes; the task's continuation needs a *pool thread* to run. When many requests are blocked, the pool is out of free threads and adds them slowly, so each request waits for a thread that is being held by another request waiting for a thread. What would let `Handle` give up its thread while it waits?
</details>
