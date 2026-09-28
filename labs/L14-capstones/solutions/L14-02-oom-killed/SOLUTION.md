# L14-02 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Retained:** ~1,200 × 100 KB `byte[]` rooted from the `MemoryCache`. **Gen2:** driven by LOH allocation.

## Root cause
LOH-sized per-request garbage *and* an unbounded cache of the whole rendered object: memory grows without bound and each allocation triggers full collections.

## Fix
Rent scratch buffers from `ArrayPool`, cache only the small answer (size-limited, with an expiry), and return the buffer. Then memory is flat and gen2 collections disappear.

## Take-aways
1. **OOM kills are usually growth, not spikes.** Look at retained memory after GC and at what keeps it (a cache).
2. Two known defects from different labs compound: LOH churn (gen2 storms) and unbounded retention.
3. Set a memory limit in the test environment and alert on `gc-heap-size` trend, not only on peak.
4. Write the post-mortem: which metric would have paged you *before* the kill?

## Extra credit
What eviction policy fits this workload: LRU/size, TTL, or both, and how would you size the limit?

## Go further
Set a container memory limit (`systemd-run --user --scope -p MemoryMax=…`) and run the slow version under load: how does it die, and after how many requests?
