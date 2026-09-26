# L09-04 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Allocations:** a 20,000-entry `Dictionary<int,int>` per request (≈ 500 KB), `ServiceProvider`/`ServiceCollection` internals.
- **Call count:** the `PriceCalculator` constructor runs once per request.

## Root cause
A new DI container per request plus a transient registration of an expensive-to-construct type: the reference data is rebuilt for every request.

## Fix
Register `PriceCalculator` as a **singleton** in the host's container (built once, thread-safe because it's read-only) and let the endpoint receive it as a parameter.

## Take-aways
1. **Never build a `ServiceProvider` on the request path** (`BuildServiceProvider` in a handler, or the service-locator pattern with new containers).
2. Match lifetime to cost and state: expensive and immutable → singleton; cheap and stateless → transient; per-request state → scoped.
3. Beware *captive dependencies*: a singleton must not hold a scoped/transient dependency that should be short-lived.
4. The DI container is fast when used as designed; the cost here was construction, not resolution.

## Extra credit
Register it as a singleton *factory* (`AddSingleton(sp => new PriceCalculator())`) and as a transient. Count constructor calls per 1,000 requests for each lifetime.

## Go further
Make the calculator scoped instead. What changes per request, and why is that still expensive? Then look at `ValidateScopes`/`ValidateOnBuild` for catching lifetime mistakes.
