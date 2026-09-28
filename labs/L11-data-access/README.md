# Lab 11: data access under load

Where the database becomes the bottleneck, and you have to spot it from query counts and timings, not from reading the LINQ and guessing what SQL it produces.

**Skills:** N+1, row explosion, connections held too long, DbContext lifetime.

**Mastery checkpoint:** Name the top SQL statement by *total* time (count × duration).

Run one: `dotnet run -c Release --project labs/L11-data-access/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 11 reading list](../../docs/READING-LIST.md#lab-11-data-access-under-load)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L11-01-orders-dashboard](exercises/L11-01-orders-dashboard/README.md) | Orders dashboard | [solution](solutions/L11-01-orders-dashboard/SOLUTION.md) |
| [L11-02-customer-profile](exercises/L11-02-customer-profile/README.md) | Customer profile | [solution](solutions/L11-02-customer-profile/SOLUTION.md) |
| [L11-03-queued-requests](exercises/L11-03-queued-requests/README.md) | Idle database, queued requests | [solution](solutions/L11-03-queued-requests/SOLUTION.md) |
| [L11-04-paged-catalogue](exercises/L11-04-paged-catalogue/README.md) | Paged catalogue | [solution](solutions/L11-04-paged-catalogue/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L11-boss-orders-service](exercises/L11-boss-orders-service/README.md) | Orders service (final boss of Lab 11) | [solution](solutions/L11-boss-orders-service/SOLUTION.md) |
