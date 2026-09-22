# L7-02 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **`dotnet-counters`:** `gc-heap-size` flat, `working-set` climbing; **dotMemory/`dumpheap -stat`:** tiny `NativeBuffer` objects, nothing large.
- **`pmap`/`smaps`:** many anonymous ~1 MB mappings.

## Root cause
`NativeBuffer` allocates unmanaged memory (`Marshal.AllocHGlobal`) but never frees it: no `Dispose`, no finalizer. The GC collects the wrapper and the pointer is lost; the block is leaked for the life of the process. This is invisible to every managed-heap tool.

## Fix
Make the wrapper `IDisposable`, free the block in `Dispose` (idempotent, via `Interlocked.Exchange`), suppress the finalizer, keep a finalizer as a safety net, and `using` at the call sites. In new code prefer a `SafeHandle`-derived type so the pattern is correct by construction.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | privateMB | kept after full GC |
|---|---|---|---|---|
| before | ≈ 33 ms | 0.04 MB | 150 | 0.00 MB |
| after | ≈ 0.8 ms | 0.04 MB | 0 | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Managed heap flat + process memory rising ⇒ unmanaged memory.** Native libraries, `Marshal.Alloc*`, memory-mapped files, thread stacks and JIT/metadata growth all live outside `gc-heap-size`.
2. Compare *managed* counters with *process* counters first; it tells you which tool family to use next (managed snapshots vs `pmap`, native profilers).
3. Ownership: whoever allocates unmanaged memory needs a release path that cannot be forgotten (`SafeHandle`, `IDisposable` + `using`, analyzers CA2000/CA1001).
4. The GC can't help: `GC.Collect()` reduces nothing here.

## Go further
Replace the class with a `SafeHandle`-derived type. What do you no longer have to write?

## Further reading
- [Kokosa et al., Pro .NET Memory Management](https://prodotnetmemory.com/)
- [Gregg, Systems Performance (the USE method)](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)
- templates/POSTMORTEM.md (this repo): write yours first
- Microsoft Learn: Implement a Dispose method; SafeHandle overview *(title only)*
- [Internals of the POH (.NET Blog)](https://devblogs.microsoft.com/dotnet/internals-of-the-poh/)
