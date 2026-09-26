# L03-02 - Solution

## What the profile shows
- **snapshots:** +40,000 `Session` and +40,000 `byte[]` (2,000 bytes). Dominator: the static `Dictionary<long, Session>` in `Sessions`.

## Root cause
`Sessions.Cache` is a static dictionary that only ever adds. Session ids are unique so nothing is ever overwritten, and lookups only reach the most recent 200 sessions, so ~99.5% of entries are never read again but stay reachable forever.

## Fix
Bound the cache. Here: an LRU with capacity 1,000 (well above the 200-session working set, so behaviour is unchanged). In a real app prefer `MemoryCache` with `SizeLimit` and expiration, or `HybridCache`, over hand-rolling; the LRU is shown so the mechanism is visible.

## Take-aways
1. **Every cache needs an eviction policy and a size bound.** If you can't say what makes an entry leave, you've written a leak.
2. Choose the bound from the *access pattern*, not a guess: measure how far back lookups actually reach.
3. Unbounded caches often start as a 'just for now' dictionary; make 'cache' a type with a capacity so it can't happen by accident.
4. A cache that's too small shows up as extra load elsewhere; watch the hit rate as well as memory.

## Extra credit
Set the capacity to 100 (below the 200-session working set). What happens to the checksum, and what does that tell you about picking a bound?

## Go further
Replace the LRU with `MemoryCache` (`SizeLimit` + `SetSize`) and compare the behaviour. Then add a TTL. What breaks if two threads call `Set` at once?
