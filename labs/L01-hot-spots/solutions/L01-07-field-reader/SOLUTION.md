# L01-07 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Sampling:** `RuntimeType.GetProperty`, `RuntimePropertyInfo.GetValue`/`Invoke`, argument checking; **allocations:** boxed `decimal`/`int`, `object[]`.

## Root cause
Per-item reflection: a property lookup and a boxed, late-bound read for every one of 600,000 reads.

## Fix
Resolve the member once per report (here a `switch` returning a typed `Func<Item, decimal>`; in general `Delegate.CreateDelegate`, compiled expression trees, or a source generator) and use the delegate in the loop.

## Take-aways
1. **Hoist the lookup, keep the read cheap.** Late binding is fine outside hot loops.
2. Reflection allocates (boxing, argument arrays) as well as burning CPU.
3. Modern options: source generators / `UnsafeAccessor` (.NET 8+) give reflection-like flexibility with direct-call speed.

## Extra credit
Cache the `PropertyInfo` but keep calling `GetValue`. How much of the win is the lookup and how much is the invoke?

## Go further
Build the getter generically with `Delegate.CreateDelegate` from the `PropertyInfo`. Compare with the `switch` and with compiled expressions.