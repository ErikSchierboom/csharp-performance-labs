# Level 11: data access under load

Where the database becomes the bottleneck, and you have to spot it from query counts and timings, not from reading the LINQ and guessing what SQL it produces.

**Skills:** N+1, row explosion, connections held too long, DbContext lifetime.

**Mastery checkpoint:** Name the top SQL statement by *total* time (count × duration).

Run one: `dotnet run -c Release --project levels/L11-data-access/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 11 reading list](../../docs/READING-LIST.md#level-11-data-access-under-load)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L11-01-endpoint-n-plus-one](exercises/L11-01-endpoint-n-plus-one/README.md) | N+1 seen from the endpoint | [solution](solutions/L11-01-endpoint-n-plus-one/SOLUTION.md) |
| [L11-03-cartesian-include](exercises/L11-03-cartesian-include/README.md) | Two collection Includes (row explosion) | [solution](solutions/L11-03-cartesian-include/SOLUTION.md) |
| [L11-04-pool-exhaustion](exercises/L11-04-pool-exhaustion/README.md) | Connection held across a slow call | [solution](solutions/L11-04-pool-exhaustion/SOLUTION.md) |
| [L11-06-dbcontext-lifetime](exercises/L11-06-dbcontext-lifetime/README.md) | DbContext lifetime (a leak in disguise) | [solution](solutions/L11-06-dbcontext-lifetime/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L11-boss-orders-service](exercises/L11-boss-orders-service/README.md) | Orders service (final boss of Level 11) | [solution](solutions/L11-boss-orders-service/SOLUTION.md) |
