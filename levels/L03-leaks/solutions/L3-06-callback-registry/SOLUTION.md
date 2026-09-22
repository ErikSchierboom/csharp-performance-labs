# L3-06 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Retention path:** static `List<Func<int>>` → `Func<int>` delegate → closure display class → `byte[20000]`.

## Root cause
A long-lived delegate's closure holds a large local (`report`) that the callback only needed one element of.

## Fix
Copy the needed value into a small local and capture that. Also: unregister callbacks when their owner is done (L3-01), and prefer `static` lambdas with explicit state to avoid accidental capture.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 3.8 ms | 57.56 MB | 57.56 MB |
| after | ≈ 2.0 ms | 57.54 MB | 0.25 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **A closure keeps everything it captures alive for as long as the delegate lives.**
2. Long-lived delegates (events, registries, timers, caches) are where this bites.
3. Capture minimal values; a `static` lambda cannot capture at all, which makes accidents a compile error.
4. Retention paths through `<>c__DisplayClass` are the fingerprint.

## Go further
Rewrite with a `static` lambda taking the value as an argument (`Func<int,int>` + state).

## Further reading
- [Ferrandez, Buggy Bits debugging labs](https://github.com/TessFerrandez/BuggyBits)
- [Teplyakov, Unusual ways of boosting up app performance: lambdas and LINQs](https://blog.jetbrains.com/dotnet/2014/07/24/unusual-ways-of-boosting-up-app-performance-lambdas-and-linqs/)
