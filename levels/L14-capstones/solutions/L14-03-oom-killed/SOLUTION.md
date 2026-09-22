# L14 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Retained:** ~1,200 × 100 KB `byte[]` rooted from the `MemoryCache`. **Gen2:** driven by LOH allocation.

## Root cause
LOH-sized per-request garbage *and* an unbounded cache of the whole rendered object: memory grows without bound and each allocation triggers full collections.

## Fix
Rent scratch buffers from `ArrayPool`, cache only the small answer (size-limited, with an expiry), and return the buffer. Then memory is flat and gen2 collections disappear.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated | gen2 / run | p99 latency | kept after full GC |
|---|---|---|---|---|---|
| before | ≈ 37 ms | 117.99 MB | 3 | ≈ 4 ms | 114.72 MB |
| after | ≈ 11 ms | 3.87 MB | 0 | ≈ 0 ms | 0.13 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **OOM kills are usually growth, not spikes.** Look at retained memory after GC and at what keeps it (a cache).
2. Two known defects from different levels compound: LOH churn (gen2 storms) and unbounded retention.
3. Set a memory limit in the test environment and alert on `gc-heap-size` trend, not only on peak.
4. Write the post-mortem: which metric would have paged you *before* the kill?

## Go further
Set a container memory limit (`systemd-run --user --scope -p MemoryMax=…`) and run the slow version under load: how does it die, and after how many requests?

## Further reading
- [Google SRE book: Monitoring Distributed Systems](https://sre.google/sre-book/monitoring-distributed-systems/)
- Nygard, Release It! *(title only)*
- templates/POSTMORTEM.md (this repo)
- [Large object heap (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap)
