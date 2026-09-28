# Lab 2: allocations & GC pressure

The code is correct and not obviously slow. It's just making the garbage collector work overtime. This is where you learn to see that happening before it shows up as latency.

**Skills:** Allocation call stacks, gen0/1/2 behaviour, boxing, spans, pooling, finalizers, LOH.

**Mastery checkpoint:** Explain why 1 GB of short-lived garbage can be cheaper than 50 MB of long-lived objects.

Run one: `dotnet run -c Release --project labs/L02-allocations/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 2 reading list](../../docs/READING-LIST.md#lab-2-allocations-gc-pressure)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L02-01-sensor-metrics](exercises/L02-01-sensor-metrics/README.md) | Sensor metrics | [solution](solutions/L02-01-sensor-metrics/SOLUTION.md) |
| [L02-02-order-lines](exercises/L02-02-order-lines/README.md) | Order lines | [solution](solutions/L02-02-order-lines/SOLUTION.md) |
| [L02-03-page-renderer](exercises/L02-03-page-renderer/README.md) | Page renderer | [solution](solutions/L02-03-page-renderer/SOLUTION.md) |
| [L02-04-tile-cache](exercises/L02-04-tile-cache/README.md) | Tile cache | [solution](solutions/L02-04-tile-cache/SOLUTION.md) |
| [L02-05-price-lookup](exercises/L02-05-price-lookup/README.md) | Price lookup | [solution](solutions/L02-05-price-lookup/SOLUTION.md) |
| [L02-06-log-classifier-2](exercises/L02-06-log-classifier-2/README.md) | Log classifier, part 2 | [solution](solutions/L02-06-log-classifier-2/SOLUTION.md) |
| [L02-07-route-stats](exercises/L02-07-route-stats/README.md) | Route stats | [solution](solutions/L02-07-route-stats/SOLUTION.md) |
| [L02-08-small-sums](exercises/L02-08-small-sums/README.md) | Small sums | [solution](solutions/L02-08-small-sums/SOLUTION.md) |
| [L02-09-case-lookup](exercises/L02-09-case-lookup/README.md) | Case lookup | [solution](solutions/L02-09-case-lookup/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L02-boss-shipment-manifest](exercises/L02-boss-shipment-manifest/README.md) | Shipment manifest | [solution](solutions/L02-boss-shipment-manifest/SOLUTION.md) |
