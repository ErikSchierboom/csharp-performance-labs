# L3-03 · Price ticker

## Symptom
After 1,000 tickers are created, used and dropped, about **20 MB is still reachable** and CPU never quite goes idle afterwards. There is no static collection and no event here. Nothing in *your* code holds the tickers.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 8 ref-ms |
| Median allocated | 49 MB |
| Kept after a full GC | ≤ 1 MB |

## Note
This exercise has a **kept-after-a-full-GC** budget (the leak gate). This one has nothing for `Reset` to clear: the leak is *not* held by a static field you can see.
In your profiler's memory-snapshot view (dotMemory's Compare, VS's Memory Usage diff, or a pair of `dotnet-gcdump` snapshots): take a snapshot, run `--profile --seconds 5`, take another, and diff them, then look at what dominates the type that grew.

## Extra credit
Create the `Timer` with `dueTime: Timeout.Infinite` in a scratch copy. Is it still a leak? Why or why not?
