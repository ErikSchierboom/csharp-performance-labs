# L2-04 · Solution

## What the profile shows
> **About the profile section.** I have not captured Rider/dotTrace/dotMemory output for this exercise. What is described is what the code and the harness numbers imply. The harness figures (time, MB, GC counts) were measured. Where your profiler shows something different, trust the profiler and note the difference in `templates/LAB-LOG.md`.

- **Harness:** gen1 = 12 per run in the exercise, 0 after the fix. Allocation MB is **identical** before and after.
- **dotTrace timeline:** the finalizer thread is busy, and GC time is high. In sampling mode, frames inside the allocator's slow path (finalizable-object registration) stand out.

## Root cause
`Tile` has a finalizer `~Tile()` copied from a "disposable pattern" snippet, but it owns no unmanaged resource: only a `byte[]`, which the GC reclaims by itself. A finalizable object is registered at allocation (slower path); when it becomes garbage it is put on the **f-reachable queue** and *survives* the collection that found it dead, is promoted to the next generation, and is finalized later by the finalizer thread. Every tile therefore costs an extra generation of survival and a trip through the finalizer thread.

## Fix
Delete the finalizer. A finalizer is only for owning an unmanaged resource directly, and even then a `SafeHandle` is the right tool. (The `Live` diagnostics counter went with it; if you need one, decrement in `Dispose()`.)

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen1 / run |
|---|---|---|---|
| before (with `~Tile()`) | ≈ 307 ms | 172 MB | 12 |
| finalizer kept, `GC.SuppressFinalize(tile)` after use | ≈ 141 ms | 172 MB | not measured |
| after (no finalizer) | ≈ 86 ms | 172 MB | 0 |

Absolute times depend on your machine and runtime version; the *ratios* and the allocation numbers should look similar.

## Take-aways
1. **Same bytes, 3.5× the time.** Allocation MB did not catch this; the time budget did. Not every regression is visible in one metric, so keep more than one.
2. `SuppressFinalize` recovers a lot (the object is no longer queued), but the allocation-time registration cost remains, so it doesn't fully match "no finalizer".
3. Rule of thumb: **if the class holds only managed memory, it needs neither a finalizer nor the full dispose pattern.** Implement `IDisposable` when you own something disposable.
4. Finalizers also run at an unpredictable time, on one thread, with no ordering guarantees. They are a last resort for cleanup, not a feature.

## Go further
Write the `IDisposable` + finalizer version that is *correct* for a real `IntPtr` native handle, then replace it with a `SafeHandle`, and compare the code size and safety.

## Further reading
- Kokosa et al., the finalization chapter
- Microsoft Learn: Implement a Dispose method; Finalizers (C# programming guide) *(title only)*
- [Fundamentals of garbage collection (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
