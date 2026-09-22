# L3-03 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory:** +1,000 `Ticker`, +1,000 `byte[]` (20,000). Retention path: GC root (timer queue / `TimerQueue`) → `TimerQueueTimer` → callback delegate → closure (`<>c__DisplayClass`) → `Ticker`.
- **Timeline:** thread-pool threads wake once a second per leaked timer.

## Root cause
Each `Ticker` starts a periodic `Timer` whose callback captures `this`. Timers are rooted by the runtime, not by your variables, so an undisposed timer keeps its callback and the `Ticker` (with its 20 KB cache) alive, and keeps firing.

## Fix
Make `Ticker` `IDisposable`, dispose the timer, and `using` the ticker. Anything that starts background activity (timers, `CancellationTokenSource`s with registrations, event subscriptions, tasks) needs a matching *stop*.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 6.8 ms | 19.33 MB | 19.33 MB |
| after | ≈ 3.3 ms | 19.33 MB | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Background activity is a root.** Timers, running tasks, registrations and event subscriptions keep their targets alive without any variable of yours pointing at them.
2. The retention path shows *what* holds it; the fix is to find what *starts* it and add the matching stop.
3. Analyzers help: CA2000 (dispose objects before losing scope) and IDE0067/CA1001 (types that own disposable fields should be disposable).
4. If the callback captures `this`, consider a static callback with state passed explicitly, so the timer doesn't root the owner (and still dispose).

## Go further
Make the callback not root the ticker (weak reference, or `static` lambda + state). Does that fix the leak without `Dispose`? What is still wrong?

## Further reading
- [Ferrandez, Buggy Bits debugging labs](https://github.com/TessFerrandez/BuggyBits)
- [Debug a memory leak tutorial (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak)
- [dotMemory Unit: assert on retained objects in a test](https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html)
- Microsoft Learn: System.Threading.Timer remarks *(title only)*
