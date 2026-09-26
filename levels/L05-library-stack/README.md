# Level 5: the library stack, single caller

The bug usually isn't in your code. It's in *how you're using* EF Core, HttpClient, JSON or the filesystem, and you have to go find it in someone else's subsystem view, not yours.

**Skills:** EF Core, HttpClient, JSON, logging, file I/O: find each from the subsystem view.

**Mastery checkpoint:** Find an N+1 from the SQL view alone, before reading any LINQ.

Run one: `dotnet run -c Release --project levels/L05-library-stack/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 5 reading list](../../docs/READING-LIST.md#level-5-library-stack-under-a-single-caller)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L05-01-customer-orders](exercises/L05-01-customer-orders/README.md) | Customer orders | [solution](solutions/L05-01-customer-orders/SOLUTION.md) |
| [L05-02-product-list](exercises/L05-02-product-list/README.md) | Product list | [solution](solutions/L05-02-product-list/SOLUTION.md) |
| [L05-03-sku-totals](exercises/L05-03-sku-totals/README.md) | SKU totals | [solution](solutions/L05-03-sku-totals/SOLUTION.md) |
| [L05-04-service-calls](exercises/L05-04-service-calls/README.md) | Service calls | [solution](solutions/L05-04-service-calls/SOLUTION.md) |
| [L05-05-json-response](exercises/L05-05-json-response/README.md) | JSON response | [solution](solutions/L05-05-json-response/SOLUTION.md) |
| [L05-06-debug-logging](exercises/L05-06-debug-logging/README.md) | Debug logging | [solution](solutions/L05-06-debug-logging/SOLUTION.md) |
| [L05-07-audit-log](exercises/L05-07-audit-log/README.md) | Audit log | [solution](solutions/L05-07-audit-log/SOLUTION.md) |
| [L05-08-byte-reader](exercises/L05-08-byte-reader/README.md) | Byte reader | [solution](solutions/L05-08-byte-reader/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L05-boss-order-report](exercises/L05-boss-order-report/README.md) | Order report | [solution](solutions/L05-boss-order-report/SOLUTION.md) |
