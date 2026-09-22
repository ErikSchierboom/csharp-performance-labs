# Level 4: async & concurrency

Code that's fast in isolation and somehow slow in practice: threads waiting on each other, locks held a beat too long, work piling up faster than it drains.

**Skills:** Thread-pool starvation, contention, back-pressure, why sampling misleads when threads wait.

**Mastery checkpoint:** Diagnose a "slow but idle CPU" service from the timeline.

Run one: `dotnet run -c Release --project levels/L04-async-concurrency/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 4 reading list](../../docs/READING-LIST.md#level-4-async-concurrency)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L4-01-sync-over-async](exercises/L4-01-sync-over-async/README.md) | Sync over async | [solution](solutions/L4-01-sync-over-async/SOLUTION.md) |
| [L4-02-latency-stats](exercises/L4-02-latency-stats/README.md) | Latency stats (lock held too long) | [solution](solutions/L4-02-latency-stats/SOLUTION.md) |
| [L4-03-fan-out](exercises/L4-03-fan-out/README.md) | Fan-out | [solution](solutions/L4-03-fan-out/SOLUTION.md) |
| [L4-04-config-cache](exercises/L4-04-config-cache/README.md) | Config cache (factory runs many times) | [solution](solutions/L4-04-config-cache/SOLUTION.md) |
| [L4-05-order-pipeline](exercises/L4-05-order-pipeline/README.md) | Order pipeline (unbounded queue) | [solution](solutions/L4-05-order-pipeline/SOLUTION.md) |
| [L4-06-reference-data](exercises/L4-06-reference-data/README.md) | Reference data (read-mostly cache) | [solution](solutions/L4-06-reference-data/SOLUTION.md) |
| [L4-07-throttled-too-tight](exercises/L4-07-throttled-too-tight/README.md) | Throttled too tight (concurrency too low) | [solution](solutions/L4-07-throttled-too-tight/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L4-boss-notification-hub](exercises/L4-boss-notification-hub/README.md) | Notification hub (final boss of Level 4) | [solution](solutions/L4-boss-notification-hub/SOLUTION.md) |
