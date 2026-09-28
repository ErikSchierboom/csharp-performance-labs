# Lab 9: the request pipeline (ASP.NET Core)

Much the same lessons as Labs 1–2, except now there's a web server and a request in the middle of every measurement, and that changes what "cost" actually means.

**Skills:** Per-request cost: middleware, DI, writing and reading bodies. Measure bytes per request.

**Mastery checkpoint:** Cut a naive endpoint's allocated bytes per request by 10× and say which middleware cost what.

Run one: `dotnet run -c Release --project labs/L09-request-pipeline/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 9 reading list](../../docs/READING-LIST.md#lab-9-the-request-pipeline)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L09-01-hello-endpoint](exercises/L09-01-hello-endpoint/README.md) | Hello endpoint | [solution](solutions/L09-01-hello-endpoint/SOLUTION.md) |
| [L09-02-audit-middleware](exercises/L09-02-audit-middleware/README.md) | Audit middleware | [solution](solutions/L09-02-audit-middleware/SOLUTION.md) |
| [L09-03-report-endpoint](exercises/L09-03-report-endpoint/README.md) | Report endpoint | [solution](solutions/L09-03-report-endpoint/SOLUTION.md) |
| [L09-04-price-endpoint](exercises/L09-04-price-endpoint/README.md) | Price endpoint | [solution](solutions/L09-04-price-endpoint/SOLUTION.md) |
| [L09-05-import-endpoint](exercises/L09-05-import-endpoint/README.md) | Import endpoint | [solution](solutions/L09-05-import-endpoint/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L09-boss-storefront-checkout](exercises/L09-boss-storefront-checkout/README.md) | Storefront checkout (final boss of L9) | [solution](solutions/L09-boss-storefront-checkout/SOLUTION.md) |
