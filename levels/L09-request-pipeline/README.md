# Level 9: the request pipeline (ASP.NET Core)

Much the same lessons as Levels 1–2, except now there's a web server and a request in the middle of every measurement, and that changes what "cost" actually means.

**Skills:** Per-request cost: middleware, DI, writing and reading bodies. Measure bytes per request.

**Mastery checkpoint:** Cut a naive endpoint's allocated bytes per request by 10× and say which middleware cost what.

Run one: `dotnet run -c Release --project levels/L09-request-pipeline/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 9 reading list](../../docs/READING-LIST.md#level-9-the-request-pipeline)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L9-01-hello-tax](exercises/L9-01-hello-tax/README.md) | Hello tax (what does a free endpoint cost?) | [solution](solutions/L9-01-hello-tax/SOLUTION.md) |
| [L9-02-pipeline-tax](exercises/L9-02-pipeline-tax/README.md) | Pipeline tax (per-request work in middleware) | [solution](solutions/L9-02-pipeline-tax/SOLUTION.md) |
| [L9-03-chatty-writer](exercises/L9-03-chatty-writer/README.md) | Chatty writer (many small writes) | [solution](solutions/L9-03-chatty-writer/SOLUTION.md) |
| [L9-04-di-lifetimes](exercises/L9-04-di-lifetimes/README.md) | DI lifetimes (a container per request) | [solution](solutions/L9-04-di-lifetimes/SOLUTION.md) |
| [L9-05-body-buffering](exercises/L9-05-body-buffering/README.md) | Request body (buffered into a string) | [solution](solutions/L9-05-body-buffering/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L9-boss-storefront-checkout](exercises/L9-boss-storefront-checkout/README.md) | Storefront checkout (final boss of L9) | [solution](solutions/L9-boss-storefront-checkout/SOLUTION.md) |
