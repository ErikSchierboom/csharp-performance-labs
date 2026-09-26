# Level 3: leaks & retention

".NET has a garbage collector, so nothing can leak": this is where that assumption breaks, and you learn to read *why* something the GC should have reclaimed is still alive.

**Skills:** GC roots, dominators, retention paths, snapshot diffing, "growing" vs "not yet collected".

**Mastery checkpoint:** From a snapshot diff alone, name the root that keeps a leaked object alive.

Run one: `dotnet run -c Release --project levels/L03-leaks/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 3 reading list](../../docs/READING-LIST.md#level-3-leaks-retention)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L03-01-event-hub](exercises/L03-01-event-hub/README.md) | Event hub | [solution](solutions/L03-01-event-hub/SOLUTION.md) |
| [L03-02-session-cache](exercises/L03-02-session-cache/README.md) | Session cache | [solution](solutions/L03-02-session-cache/SOLUTION.md) |
| [L03-03-price-ticker](exercises/L03-03-price-ticker/README.md) | Price ticker | [solution](solutions/L03-03-price-ticker/SOLUTION.md) |
| [L03-04-batch-report](exercises/L03-04-batch-report/README.md) | Batch report | [solution](solutions/L03-04-batch-report/SOLUTION.md) |
| [L03-05-tag-registry](exercises/L03-05-tag-registry/README.md) | Tag registry | [solution](solutions/L03-05-tag-registry/SOLUTION.md) |
| [L03-06-callback-registry](exercises/L03-06-callback-registry/README.md) | Callback registry | [solution](solutions/L03-06-callback-registry/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L03-boss-session-gateway](exercises/L03-boss-session-gateway/README.md) | Session gateway | [solution](solutions/L03-boss-session-gateway/SOLUTION.md) |
