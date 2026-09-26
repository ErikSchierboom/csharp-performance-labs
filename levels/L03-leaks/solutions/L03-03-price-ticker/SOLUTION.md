# L03-03 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **snapshots::** +1,000 `Ticker`, +1,000 `byte[]` (20,000). Retention path: GC root (timer queue / `TimerQueue`) → `TimerQueueTimer` → callback delegate → closure (`<>c__DisplayClass`) → `Ticker`.
- **timeline:** thread-pool threads wake once a second per leaked timer.

## Root cause
Each `Ticker` starts a periodic `Timer` whose callback captures `this`. Timers are rooted by the runtime, not by your variables, so an undisposed timer keeps its callback and the `Ticker` (with its 20 KB cache) alive, and keeps firing.

## Fix
Make `Ticker` `IDisposable`, dispose the timer, and `using` the ticker. Anything that starts background activity (timers, `CancellationTokenSource`s with registrations, event subscriptions, tasks) needs a matching *stop*.

## Take-aways
1. **Background activity is a root.** Timers, running tasks, registrations and event subscriptions keep their targets alive without any variable of yours pointing at them.
2. The retention path shows *what* holds it; the fix is to find what *starts* it and add the matching stop.
3. Analyzers help: CA2000 (dispose objects before losing scope) and IDE0067/CA1001 (types that own disposable fields should be disposable).
4. If the callback captures `this`, consider a static callback with state passed explicitly, so the timer doesn't root the owner (and still dispose).

## Extra credit
Create the `Timer` with `dueTime: Timeout.Infinite` in a scratch copy. Is it still a leak? Why or why not?

## Go further
Make the callback not root the ticker (weak reference, or `static` lambda + state). Does that fix the leak without `Dispose`? What is still wrong?
