# L3-01 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory compare:** 2,000 more `Widget` and 2,000 more `byte[]` (10,000 bytes). Retention path: `Hub` (static field) → event delegate (`MulticastDelegate` invocation list) → `Widget`.
- **dotTrace:** publish time grows with each run: more subscribers to invoke; each `+=` also copies the whole invocation list.

## Root cause
The hub lives for the whole process; each `Widget` subscribes with `hub.Published += OnMessage`, which stores a delegate pointing at the widget in the hub's list. Nothing ever removes it, so the hub (a GC root via a static field) keeps every widget and its 10 KB buffer alive. This is the classic **event-handler leak**; it is also a slow leak in CPU, since every publish now invokes every dead widget.

## Fix
Make `Widget` `IDisposable`, unsubscribe in `Dispose` (`-=`), and `using` it. A safer general shape: a weak-event pattern, or have the hub hand out a subscription token that unsubscribes when disposed.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 14 ms | 19.46 MB | 19.32 MB |
| after | ≈ 1.9 ms | 19.44 MB | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Events are references.** The publisher holds every subscriber for as long as the publisher lives.
2. Symptoms: memory that only ever goes up in step with *object creation*, plus operations that get slower over time.
3. Lambdas make it worse: `hub.Published += x => ...this...` cannot be unsubscribed without keeping the delegate in a variable.
4. The kept-after-full-GC gate is the leak test: a healthy workload leaves about nothing behind.

## Go further
Implement the subscription-token version (`IDisposable Subscribe(Action<int>)`). Then write the weak-event version with `WeakReference<T>` and list its downsides (who cleans up the dead entries?).

## Further reading
- [Ferrandez, Buggy Bits debugging labs](https://github.com/TessFerrandez/BuggyBits)
- [Debug a memory leak tutorial (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak)
- [dotMemory Unit: assert on retained objects in a test](https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html)
- Kokosa et al., Pro .NET Memory Management: GC roots and delegates
