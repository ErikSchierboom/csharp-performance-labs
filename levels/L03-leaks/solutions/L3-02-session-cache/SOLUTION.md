# L3-02 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **dotMemory compare:** +40,000 `Session` and +40,000 `byte[]` (2,000 bytes). Dominator: the static `Dictionary<long, Session>` in `Sessions`.

## Root cause
`Sessions.Cache` is a static dictionary that only ever adds. Session ids are unique so nothing is ever overwritten, and lookups only reach the most recent 200 sessions, so ~99.5% of entries are never read again but stay reachable forever.

## Fix
Bound the cache. Here: an LRU with capacity 1,000 (well above the 200-session working set, so behaviour is unchanged). In a real app prefer `MemoryCache` with `SizeLimit` and expiration, or `HybridCache`, over hand-rolling; the LRU is shown so the mechanism is visible.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | kept after full GC |
|---|---|---|---|
| before | ≈ 5.4 ms | 78.43 MB | 78.43 MB |
| after | ≈ 7.1 ms | 80.57 MB | 2.01 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Every cache needs an eviction policy and a size bound.** If you can't say what makes an entry leave, you've written a leak.
2. Choose the bound from the *access pattern*, not a guess: measure how far back lookups actually reach.
3. Unbounded caches often start as a 'just for now' dictionary; make 'cache' a type with a capacity so it can't happen by accident.
4. A cache that's too small shows up as extra load elsewhere; watch the hit rate as well as memory.

## Go further
Replace the LRU with `MemoryCache` (`SizeLimit` + `SetSize`) and compare the behaviour. Then add a TTL. What breaks if two threads call `Set` at once?

## Further reading
- [Ferrandez, Buggy Bits debugging labs](https://github.com/TessFerrandez/BuggyBits)
- [Debug a memory leak tutorial (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak)
- [dotMemory Unit: assert on retained objects in a test](https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html)
- Microsoft Learn: Cache in-memory in ASP.NET Core (MemoryCache SizeLimit) *(title only)*
