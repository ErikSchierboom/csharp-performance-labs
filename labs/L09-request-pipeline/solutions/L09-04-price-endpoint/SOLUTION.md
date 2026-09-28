# L09-04 - Solution

## What the profile shows
- **Allocations:** ~600 KB per request, almost all from `PriceCatalog`'s constructor: `string.Split` results (one `string[]` and two strings per CSV row), and a `Dictionary<int, decimal>` resized up to 2,000 entries.
- **Call count:** `PriceCatalog..ctor` runs once per request: 1,500 times per run, for data that never changes.

## Root cause
`PriceCatalog` is registered as **transient**, so the container creates a new one every time something asks for it. `PricingService` is resolved for every request, and it depends on `PriceCatalog`, so every request parses the whole price file into a new dictionary, uses one entry, and throws it away.

Nothing looks wrong in the code: `AddTransient` is what most samples use, and the constructor "just loads the prices". The cost only shows once you notice *how often* that constructor runs.

## Fix
`builder.Services.AddSingleton<PriceCatalog>();` One word. The catalog is built on first use and shared by every request. That's safe because it's read-only after construction (a `Dictionary` is safe for concurrent reads as long as nobody writes). `PricingService` can stay transient: it's cheap, and a transient may depend on a singleton.

## Take-aways
1. **Match the lifetime to the cost and the state.** Expensive to build and immutable -> singleton. Cheap and stateless -> transient (or singleton). Holds per-request state (a `DbContext`, the current user) -> scoped.
2. **Transient is contagious in cost:** resolving a transient service also builds every transient it depends on, however deep.
3. **Count constructor calls** for anything that loads data: a constructor running once per request for data that never changes is this bug.
4. Watch the reverse mistake, *captive dependencies*: a singleton that depends on a scoped service keeps one instance of it forever. `ValidateScopes` catches that in development.

## Extra credit
Register `PriceCatalog` as **scoped** instead. How many times is it built per 1,500 requests, and why doesn't that fix it? Then make `PricingService` a singleton and leave `PriceCatalog` transient. Why does that *also* pass, and why is it a worse fix?

## Go further
The prices do change occasionally in real life. How would you reload the singleton without restarting the app, and without a request ever seeing a half-built table? (Look at swapping a reference with `Volatile.Write`/`Interlocked.Exchange`, or `IOptionsMonitor`.)
