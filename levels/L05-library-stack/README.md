# Level 5: the library stack, single caller

The bug usually isn't in your code. It's in *how you're using* EF Core, HttpClient, JSON or the filesystem, and you have to go find it in someone else's subsystem view, not yours.

**Skills:** EF Core, HttpClient, JSON, logging, file I/O: find each from the subsystem view.

**Mastery checkpoint:** Find an N+1 from the SQL view alone, before reading any LINQ.

Run one: `dotnet run -c Release --project levels/L05-library-stack/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 5 reading list](../../docs/READING-LIST.md#level-5-library-stack-under-a-single-caller)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L5-01-orders-n-plus-one](exercises/L5-01-orders-n-plus-one/README.md) | Orders (N+1) | [solution](solutions/L5-01-orders-n-plus-one/SOLUTION.md) |
| [L5-02-product-list](exercises/L5-02-product-list/README.md) | Product list (tracking & over-fetching) | [solution](solutions/L5-02-product-list/SOLUTION.md) |
| [L5-03-sku-totals](exercises/L5-03-sku-totals/README.md) | SKU totals (missing index) | [solution](solutions/L5-03-sku-totals/SOLUTION.md) |
| [L5-04-http-client-per-call](exercises/L5-04-http-client-per-call/README.md) | HttpClient per call | [solution](solutions/L5-04-http-client-per-call/SOLUTION.md) |
| [L5-05-json-response](exercises/L5-05-json-response/README.md) | JSON response | [solution](solutions/L5-05-json-response/SOLUTION.md) |
| [L5-06-log-noise](exercises/L5-06-log-noise/README.md) | Log noise (disabled logging isn't free) | [solution](solutions/L5-06-log-noise/SOLUTION.md) |
| [L5-07-audit-log](exercises/L5-07-audit-log/README.md) | Audit log (per-call file I/O) | [solution](solutions/L5-07-audit-log/SOLUTION.md) |
| [L5-08-byte-reader](exercises/L5-08-byte-reader/README.md) | Byte reader (unbuffered file reads) | [solution](solutions/L5-08-byte-reader/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L5-boss-order-report](exercises/L5-boss-order-report/README.md) | Order report (final boss of Level 5) | [solution](solutions/L5-boss-order-report/SOLUTION.md) |
