# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Count concurrent background jobs (`peakConcurrentJobs`) and watch thread-pool thread count and queue length in `dotnet-counters` during the burst.
</details>

<details><summary>Hint 2: where?</summary>

How many jobs can run simultaneously? What in the code limits it?
</details>

<details><summary>Hint 3: why?</summary>

`Task.Run` per request is a hidden **unbounded queue on the shared thread pool**: nothing bounds concurrency, and blocking jobs steal threads from request handling. Put a **bounded queue** (a `Channel`) in front of a **fixed number of workers** (a `BackgroundService` in a real app); accept quickly, process at a rate the system can afford, and apply back-pressure or shed load if the queue fills.
</details>
