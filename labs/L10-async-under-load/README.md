# Lab 10: async, threads & the pool under load

Lab 4's lessons again, except now there's real concurrent load hitting the thread pool instead of one call at a time, which is exactly when those problems actually bite.

**Skills:** Starvation, blocking, fan-out, cancellation, background work.

**Mastery checkpoint:** From a latency-vs-concurrency graph and the pool counters, predict the bug before opening the code.

Run one: `dotnet run -c Release --project labs/L10-async-under-load/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 10 reading list](../../docs/READING-LIST.md#lab-10-async-threads-the-pool-under-load)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L10-01-database-endpoint](exercises/L10-01-database-endpoint/README.md) | Database-backed endpoint | [solution](solutions/L10-01-database-endpoint/SOLUTION.md) |
| [L10-02-async-handler](exercises/L10-02-async-handler/README.md) | Async handler | [solution](solutions/L10-02-async-handler/SOLUTION.md) |
| [L10-03-aggregation-endpoint](exercises/L10-03-aggregation-endpoint/README.md) | Aggregation endpoint | [solution](solutions/L10-03-aggregation-endpoint/SOLUTION.md) |
| [L10-04-currency-conversion](exercises/L10-04-currency-conversion/README.md) | Currency conversion | [solution](solutions/L10-04-currency-conversion/SOLUTION.md) |
| [L10-05-impatient-clients](exercises/L10-05-impatient-clients/README.md) | Impatient clients | [solution](solutions/L10-05-impatient-clients/SOLUTION.md) |
| [L10-06-background-jobs](exercises/L10-06-background-jobs/README.md) | Background jobs | [solution](solutions/L10-06-background-jobs/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L10-boss-async-quotes](exercises/L10-boss-async-quotes/README.md) | Async quotes (final boss of Lab 10) | [solution](solutions/L10-boss-async-quotes/SOLUTION.md) |
