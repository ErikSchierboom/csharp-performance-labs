# L04-02 - Solution

## What the profile shows

- **Timeline:** 7 of 8 threads blocked on the monitor at any moment; contention count in the hundreds of thousands.
- **Sampling:** `Audit` dominates self time, and the total CPU is ≈ 1 core.

## Root cause
The lock's critical section includes the expensive `Audit` call, which only needs its argument. All threads take turns doing that work, so parallel workers behave like one thread plus lock overhead (and extra context switching when the lock is contended).

## Fix
Shrink the critical section to the smallest thing that must be atomic: compute `Audit` outside, then update the shared bucket with `Interlocked.Add` (no lock). Other options: per-thread buckets merged at the end (no sharing at all) or striped locks.

## Take-aways
1. **Hold locks for the shortest possible time**, and never across work that doesn't touch shared state (or across I/O or `await`).
2. Contention shows as *waiting*, not CPU: low CPU with many threads means look at the timeline and lock counters.
3. `Interlocked` and per-thread accumulation beat locks for counters; measure, because false sharing (Lab 6) can bite the striped version.
4. Correctness first: the checksum verifies the atomic version still adds up.

## Extra credit
Keep the lock but move `Audit` outside it. How much of the win do you get? What's left, and why?

## Go further
Implement per-thread buckets (`ThreadLocal<long[]>` or `Parallel.For` with local state) merged at the end. Compare it with `Interlocked` at 8 and at 32 workers.
