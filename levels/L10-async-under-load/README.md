# Level 10: async, threads & the pool under load

Level 4's lessons again, except now there's real concurrent load hitting the thread pool instead of one call at a time, which is exactly when those problems actually bite.

**Skills:** Starvation, blocking, fan-out, cancellation, background work.

**Mastery checkpoint:** From a latency-vs-concurrency graph and the pool counters, predict the bug before opening the code.

Run one: `dotnet run -c Release --project levels/L10-async-under-load/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 10 reading list](../../docs/READING-LIST.md#level-10-async-threads-the-pool-under-load)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L10-01-sync-over-async](exercises/L10-01-sync-over-async/README.md) | Sync over async (in an endpoint) | [solution](solutions/L10-01-sync-over-async/SOLUTION.md) |
| [L10-02-blocking-in-async](exercises/L10-02-blocking-in-async/README.md) | Blocking inside an async handler | [solution](solutions/L10-02-blocking-in-async/SOLUTION.md) |
| [L10-03-endpoint-fan-out](exercises/L10-03-endpoint-fan-out/README.md) | Endpoint fan-out | [solution](solutions/L10-03-endpoint-fan-out/SOLUTION.md) |
| [L10-04-rate-gate](exercises/L10-04-rate-gate/README.md) | Async lock around a cacheable call | [solution](solutions/L10-04-rate-gate/SOLUTION.md) |
| [L10-05-cancellation](exercises/L10-05-cancellation/README.md) | Cancellation (work after the client left) | [solution](solutions/L10-05-cancellation/SOLUTION.md) |
| [L10-06-fire-and-forget](exercises/L10-06-fire-and-forget/README.md) | Fire and forget (unbounded background work) | [solution](solutions/L10-06-fire-and-forget/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L10-boss-async-quotes](exercises/L10-boss-async-quotes/README.md) | Async quotes (final boss of Level 10) | [solution](solutions/L10-boss-async-quotes/SOLUTION.md) |
