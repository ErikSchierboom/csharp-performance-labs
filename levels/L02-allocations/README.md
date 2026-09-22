# Level 2: allocations & GC pressure

The code is correct and not obviously slow. It's just making the garbage collector work overtime. This is where you learn to see that happening before it shows up as latency.

**Skills:** Allocation call stacks, gen0/1/2 behaviour, boxing, spans, pooling, finalizers, LOH.

**Mastery checkpoint:** Explain why 1 GB of short-lived garbage can be cheaper than 50 MB of long-lived objects.

Run one: `dotnet run -c Release --project levels/L02-allocations/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 2 reading list](../../docs/READING-LIST.md#level-2-allocations-gc-pressure)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L2-01-metrics-boxing](exercises/L2-01-metrics-boxing/README.md) | Metrics boxing | [solution](solutions/L2-01-metrics-boxing/SOLUTION.md) |
| [L2-02-order-lines](exercises/L2-02-order-lines/README.md) | Order lines | [solution](solutions/L2-02-order-lines/SOLUTION.md) |
| [L2-03-page-renderer](exercises/L2-03-page-renderer/README.md) | Page renderer | [solution](solutions/L2-03-page-renderer/SOLUTION.md) |
| [L2-04-tile-cache](exercises/L2-04-tile-cache/README.md) | Tile cache | [solution](solutions/L2-04-tile-cache/SOLUTION.md) |
| [L2-05-price-lookup](exercises/L2-05-price-lookup/README.md) | Price lookup | [solution](solutions/L2-05-price-lookup/SOLUTION.md) |
| [L2-06-log-classifier-2](exercises/L2-06-log-classifier-2/README.md) | Log classifier, part 2 | [solution](solutions/L2-06-log-classifier-2/SOLUTION.md) |
| [L2-07-route-stats](exercises/L2-07-route-stats/README.md) | Route stats | [solution](solutions/L2-07-route-stats/SOLUTION.md) |
| [L2-08-small-sums](exercises/L2-08-small-sums/README.md) | Small sums (interface enumeration) | [solution](solutions/L2-08-small-sums/SOLUTION.md) |
| [L2-09-case-lookup](exercises/L2-09-case-lookup/README.md) | Case lookup (ToLower as a key) | [solution](solutions/L2-09-case-lookup/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L2-boss-shipment-manifest](exercises/L2-boss-shipment-manifest/README.md) | Shipment manifest (final boss of Level 2) | [solution](solutions/L2-boss-shipment-manifest/SOLUTION.md) |
