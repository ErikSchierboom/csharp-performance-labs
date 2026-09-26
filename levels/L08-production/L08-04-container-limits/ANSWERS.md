# Answers (measured)

> Measured with `systemd-run --user --scope -p MemoryMax=… -p MemorySwapMax=0` on a 20-core Linux box, .NET 10, workstation non-concurrent GC, live set 150 MB, 6–8 s runs. "Allocations" = total short-lived buffers allocated in the run (throughput proxy). Yours will differ in absolute terms.

| Limit / setting | Result | `limit=` seen | Allocations (6 s run) |
|---|---|---|---|
| none (baseline) | fine | 33,486 MB (host RAM) | ≈ 10–14 M (8 s: 14.0 M) |
| 260M | fine | ≈ 235 MB | 10.2 M |
| 240M | fine | – | 9.5 M |
| 220M | fine, **~30% slower** | – | 6.9 M |
| 200M | **crashes: "Out of memory", exit 134 (abort)** | – | – |
| 220M + `GCHeapHardLimit=0xC800000` (200 MB) | fine, **throughput restored** | – | 10.0 M |
| 220M + `GCConserveMemory=9` | fine, in between | – | 8.6 M |

1. The runtime reads the **cgroup memory limit** and sets a default **heap hard limit of 75% of it** (300M → 235 MB here, visible as `limit=` from `GC.GetGCMemoryInfo().TotalAvailableMemoryBytes`).
2. With the default cap of 75% of 220 MB ≈ 173 MB and 150 MB live, there's only ~23 MB of headroom for garbage, so the GC runs far more often and works harder to stay under the cap: throughput falls ~30%.
3. Below the point where live data plus a minimum working set fits under the GC's *own* cap (75% of 200 MB = 157 MB vs 150 MB live), the **GC hits its hard limit and throws/aborts with OutOfMemory before the kernel's OOM killer acts**. In other setups you'd see the kernel kill (exit 137).
4. An explicit `GCHeapHardLimit` of 200 MB leaves more room for the heap and less for everything else in the container (runtime, JIT, thread stacks, native libraries). The risk: the *kernel* OOM-kills the process once total usage exceeds the container limit, with no warning and no managed exception. Leave headroom for non-heap memory and measure the process's working set, not just the GC heap.
5. For a 256 MB limit and a 150 MB live set: raise the *headroom for the GC* (explicit heap limit around 75–80% of the limit or `GCHeapHardLimitPercent`), reduce the working set (pooling, smaller buffers, workstation GC), or raise the container limit. Confirm with the same load test at the target limit: throughput, `gc-heap-size`, GC pause time, and no OOM over hours.

## The lesson
A memory limit isn't a cliff you either fit under or don't: as you approach it the GC gets busier long before anything fails. Test **at** the limit, not just below it.

## Reveal
The service keeps 150 × 1 MB arrays alive and allocates 64 KB short-lived arrays on 4 threads as fast as it can.
