# L10-01 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Counters:** thread-pool queue length > 0 and thread count growing, CPU near idle. **Timeline:** threads in `Task.Wait`/`GetResult`.

## Root cause
`.Result` blocks a pool thread for the duration of each call, exhausting the pool under concurrency (thread-pool starvation).

## Fix
`async`/`await` in the handler; minimal APIs and MVC both await your `Task`, so nothing blocks.

## Take-aways
1. **The web server's throughput is limited by *blocked threads*, not CPU.** Async handlers scale with concurrency; blocking handlers scale with thread count.
2. Sync-over-async in one hot endpoint can starve the *whole* server, including unrelated endpoints (health checks time out too).
3. Same signature as L04-01: idle CPU, queueing, growing thread count: read the counters, not the CPU graph.
4. Don't fix with `SetMinThreads`; fix the blocking.

## Extra credit
Increase users to 400. How do the two versions' p99s scale?

## Go further
Add a second endpoint `/health` that returns immediately. Measure its p99 while the slow endpoint is under load, in both versions.
