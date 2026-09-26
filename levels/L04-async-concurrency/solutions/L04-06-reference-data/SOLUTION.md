# L04-06 - Solution

## What the profile shows

- **Timeline:** ~50% time waiting; lock contention high.
- **Sampling:** time inside `Monitor.Enter`/`TryEnter` slow path.

## Root cause
A single hot lock on a read-dominated path: even a tiny critical section becomes a serialisation point when many threads take it back-to-back (lock convoy, cache-line ping-pong on the lock word).

## Fix
Copy-on-write snapshot: readers read a `volatile` reference to an immutable-by-convention dictionary (lock-free); the rare writer copies, changes, and swaps the reference under a writer-only lock. Cost: each write copies the whole map: fine for 1,000 entries and a few writes; wrong for large maps with frequent writes.

## Take-aways
1. **Read-mostly data wants lock-free reads.** Copy-on-write, `ImmutableDictionary`, or `FrozenDictionary` (rebuilt on change) are the usual tools.
2. `ReaderWriterLockSlim` is *not* an automatic fix: for very short critical sections its own overhead can be worse than a plain `lock`. Measure it (extra credit).
3. The write path pays: know the write rate and the object size before choosing.
4. Publication needs `volatile` (or `Volatile.Write`/`Interlocked.Exchange`) so readers see a fully-built object.

## Extra credit
Try `ReaderWriterLockSlim` instead. Does it beat the `lock`? Beat copy-on-write? Explain the ordering you observe.

## Go further
Use `FrozenDictionary` rebuilt on write, and compare read speed. What if writes were 10% of operations instead of 0.005%?
