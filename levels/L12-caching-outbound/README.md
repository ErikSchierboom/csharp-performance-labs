# Level 12: caching & outbound calls

Caches and outbound calls that work fine at low traffic and fall over the moment two things happen at once, which, at real scale, is always.

**Skills:** Stampedes, unbounded caches, HttpClient lifetime, output caching, retry storms.

**Mastery checkpoint:** Explain what happens at 2× capacity and pick the mechanism that degrades gracefully.

Run one: `dotnet run -c Release --project levels/L12-caching-outbound/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 12 reading list](../../docs/READING-LIST.md#level-12-caching-outbound-calls)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L12-01-downstream-call](exercises/L12-01-downstream-call/README.md) | Downstream call | [solution](solutions/L12-01-downstream-call/SOLUTION.md) |
| [L12-02-popular-items](exercises/L12-02-popular-items/README.md) | Popular items | [solution](solutions/L12-02-popular-items/SOLUTION.md) |
| [L12-03-search-results](exercises/L12-03-search-results/README.md) | Search results | [solution](solutions/L12-03-search-results/SOLUTION.md) |
| [L12-04-report-service](exercises/L12-04-report-service/README.md) | Report service | [solution](solutions/L12-04-report-service/SOLUTION.md) |
| [L12-05-flaky-downstream](exercises/L12-05-flaky-downstream/README.md) | Flaky downstream | [solution](solutions/L12-05-flaky-downstream/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L12-boss-catalog-service](exercises/L12-boss-catalog-service/README.md) | Catalog service (final boss of Level 12) | [solution](solutions/L12-boss-catalog-service/SOLUTION.md) |
