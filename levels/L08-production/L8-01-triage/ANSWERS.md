# Answers (measured)

> Measured on a 20-core Linux box, .NET 10, `dotnet-counters collect` with a 2-second refresh, averaged over ~8 s. Absolute numbers depend on your machine; the *signatures* should match.

| Scenario | Diagnosis | What the counters showed |
|---|---|---|
| **a** | **CPU-bound** | CPU ≈ **4.0 cores** (4 busy threads); GC idle (no gen2, no pauses); no pool queue; no contention |
| **b** | **GC pressure** (large-object churn) | ~**400 gen2 collections per 2 s**; GC pause ≈ 0.23 s per 2 s (~11% of wall time); allocation ≈ 4 GB per 2 s; CPU only ≈ 0.5 core |
| **c** | **Thread-pool starvation** (sync-over-async) | CPU ≈ **0.1 core** (almost idle); pool **queue length > 0** (≈ 3) and thread count ≈ 13 and *growing*; a few lock contentions from `Task.Wait` |
| **d** | **Lock contention** | CPU ≈ 0.04 core; **~20 lock contentions per 2 s**; *no* pool activity (it uses dedicated threads) |
| **e** | **Healthy / idle** | CPU ≈ 0.05 core; nothing else moving: don't fix it |

## Question 1: which two look alike, and what separates them?
**c and d** (both idle CPU, slow work). Thread-pool queue length / thread count moves only in **c**; `lock_contentions` is the signal in **d**. (c also shows a few contentions, from `Task.Wait` internals; it's the *pool* counters that identify it.)

## Question 2: which would a CPU sampling profile fail to explain?
**c and d** (and **e**, which is fine): threads that are *waiting* aren't on CPU, so a CPU profile is nearly empty. Use the **timeline/trace** view or thread stacks (`dotnet-stack`/`dotnet-dump analyze`, `clrstack`) to see what they wait on.

## Question 3: which would you not fix?
**e**. Low utilisation and no saturation on any resource is not a problem; chasing it wastes time (and the USE method says: no resource is saturated, there are no errors).

## Question 4: next tool
| Scenario | Next tool | What you hope to see |
|---|---|---|
| a | `dotnet-trace` (sampling) → flame graph | the hot method (this is Lab L8-02) |
| b | `dotnet-counters` LOH size + `dotnet-gcdump`/allocation trace | who allocates the large arrays; then pool/reuse them (Level 2) |
| c | `dotnet-stack`/dump `clrstack` on pool threads | many threads in `Task.Wait`/`GetResult`: find and remove the sync-over-async (Level 4) |
| d | dump `syncblk`/`threads` | one thread holding the monitor while others wait; shrink the critical section (Level 4) |

## The habit this practises
Classify by **resource** (USE: utilisation, saturation, errors) first; pick the heavier tool second.

## Reveal
`a` busy loops on 4 threads · `b` allocates 100–300 KB arrays constantly · `c` bursts of 200 `Task.Run(() => asyncCall().Result)` with the pool pinned to 4 threads · `d` 16 threads taking one lock and sleeping 2 ms inside it · `e` 200 async loops awaiting `Task.Delay`.
