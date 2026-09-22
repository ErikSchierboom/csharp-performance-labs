# Level 12: caching & outbound calls

Caches and outbound calls that work fine at low traffic and fall over the moment two things happen at once, which, at real scale, is always.

**Skills:** Stampedes, unbounded caches, HttpClient lifetime, output caching, retry storms.

**Mastery checkpoint:** Explain what happens at 2× capacity and pick the mechanism that degrades gracefully.

Run one: `dotnet run -c Release --project levels/L12-caching-outbound/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 12 reading list](../../docs/READING-LIST.md#level-12-caching-outbound-calls)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L12-01-client-lifetime](exercises/L12-01-client-lifetime/README.md) | HttpClient created per request | [solution](solutions/L12-01-client-lifetime/SOLUTION.md) |
| [L12-02-cache-stampede](exercises/L12-02-cache-stampede/README.md) | Cache stampede | [solution](solutions/L12-02-cache-stampede/SOLUTION.md) |
| [L12-03-unbounded-memory-cache](exercises/L12-03-unbounded-memory-cache/README.md) | Unbounded MemoryCache | [solution](solutions/L12-03-unbounded-memory-cache/SOLUTION.md) |
| [L12-04-output-cache](exercises/L12-04-output-cache/README.md) | Cacheable responses computed every time | [solution](solutions/L12-04-output-cache/SOLUTION.md) |
| [L12-05-retry-storm](exercises/L12-05-retry-storm/README.md) | Retry storm | [solution](solutions/L12-05-retry-storm/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L12-boss-catalog-service](exercises/L12-boss-catalog-service/README.md) | Catalog service (final boss of Level 12) | [solution](solutions/L12-boss-catalog-service/SOLUTION.md) |
