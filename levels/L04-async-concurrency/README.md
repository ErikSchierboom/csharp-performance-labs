# Level 4: async & concurrency

Code that's fast in isolation and somehow slow in practice: threads waiting on each other, locks held a beat too long, work piling up faster than it drains.

**Skills:** Thread-pool starvation, contention, back-pressure, why sampling misleads when threads wait.

**Mastery checkpoint:** Diagnose a "slow but idle CPU" service from the timeline.

Run one: `dotnet run -c Release --project levels/L04-async-concurrency/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 4 reading list](../../docs/READING-LIST.md#level-4-async-concurrency)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L04-01-request-burst](exercises/L04-01-request-burst/README.md) | Request burst | [solution](solutions/L04-01-request-burst/SOLUTION.md) |
| [L04-02-latency-stats](exercises/L04-02-latency-stats/README.md) | Latency stats | [solution](solutions/L04-02-latency-stats/SOLUTION.md) |
| [L04-03-bulk-fetch](exercises/L04-03-bulk-fetch/README.md) | Bulk fetch | [solution](solutions/L04-03-bulk-fetch/SOLUTION.md) |
| [L04-04-config-cache](exercises/L04-04-config-cache/README.md) | Config cache | [solution](solutions/L04-04-config-cache/SOLUTION.md) |
| [L04-05-order-pipeline](exercises/L04-05-order-pipeline/README.md) | Order pipeline | [solution](solutions/L04-05-order-pipeline/SOLUTION.md) |
| [L04-06-reference-data](exercises/L04-06-reference-data/README.md) | Reference data | [solution](solutions/L04-06-reference-data/SOLUTION.md) |
| [L04-07-partner-api-calls](exercises/L04-07-partner-api-calls/README.md) | Partner API calls | [solution](solutions/L04-07-partner-api-calls/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L04-boss-notification-hub](exercises/L04-boss-notification-hub/README.md) | Notification hub | [solution](solutions/L04-boss-notification-hub/SOLUTION.md) |
