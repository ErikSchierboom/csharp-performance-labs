# L01-07 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Sampling:** `RuntimeType.GetProperty`, `RuntimePropertyInfo.GetValue`/`Invoke`, argument checking; **allocations:** boxed `decimal`/`int`, `object[]`.

## Root cause
Per-item reflection: a property lookup and a boxed, late-bound read for every one of 600,000 reads.

## Fix
Directly use the class' properties in a single loop.

## Take-aways
1. Reflection allocates (boxing, argument arrays) as well as burning CPU.
2. Don't overcomplicate; direct member access is much faster than reflection.
3. Merge the three loops into a single loop.

## Go further
Build the getter generically with `Delegate.CreateDelegate` from the `PropertyInfo`. Compare with the `switch` and with compiled expressions.
