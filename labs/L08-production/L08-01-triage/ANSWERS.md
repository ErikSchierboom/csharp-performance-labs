# Answers


| Scenario | Diagnosis | What the counters showed |
|---|---|---|
| **a** | **CPU-bound** | `dotnet.process.cpu.time` with `cpu.mode=user` ≈ 4 per 1 s interval ≈ 4 cores busy (out of `cpu.count` = 20, so ~20% of the machine);<br/>`cpu.time[mode=system]` ≈ 0. GC idle (0 collections in every generation, no pauses);<br/> thread-pool counters all 0 (the busy threads must be dedicated `Thread`s, not pool threads); |
| **b** | **GC pressure** | ~**200–250 gen2 collections per second**<br/>~0 gen0/gen1: every collection is a gen2 because the LOH triggers it);<br/>GC pause = 0.08–0.09 s per second (~8-9% of wall time);<br/> allocation ≈ 2-3 GB per s;<br/> CPU only ≈ 0.4–0.5 core |
| **c** | **Thread-pool starvation** | CPU ≈ **0.1 core** (almost idle);<br/>pool **queue length ≈ 70-180** that doesn't drain;<br/>pool **thread count ≈ 80 -> 200 and climbing** (the meter shows this as about +10 threads per 2 s);<br/>~10–30 lock contentions per 2 s from `Task.Wait` internals, the **same range as d**. After ~25 s the pool has injected enough threads (~200) that the starvation clears: the queue drains, CPU and throughput jump. **Measure within the first 20 s.** |
| **d** | **Lock contention** | CPU ≈ 0.05 core;<br/>**~20 lock contentions per 2 s**, steady; *no* pool activity at all: 0 threads, 0 queue, 0 work items (it uses dedicated threads) |
| **e** | **Healthy** | CPU ≈ 0.05 core;<br/>~14 pool threads, **queue 0**, ~2000 work items/s completing smoothly;<br/>no GC;<br/>an occasional stray contention. Nothing saturated: don't fix it |

## Question 1: which two look alike, and what separates them?
**c and d** (both idle CPU, slow work, and *both* show lock contentions at a similar rate, so `lock_contentions` does **not** separate them). The separating counter is the **thread pool**: in **c** the queue length is large and the thread count keeps climbing. In **d** every pool counter is flat at 0, because the contending threads are dedicated `Thread`s. So c is "pool counters + contentions" and d is "contentions only".

## Question 2: which would a CPU sampling profile fail to explain?
**c and d** (and **e**, which is fine): threads that are *waiting* aren't on CPU, so a CPU profile is nearly empty. Use the **timeline/trace** view or thread stacks (`dotnet-stack`/`dotnet-dump analyze`, `clrstack`) to see what they wait on.

## Question 3: which would you not fix?
**e**. Low utilisation and no saturation on any resource is not a problem; chasing it wastes time (and the USE method says: no resource is saturated, there are no errors).

## Question 4: next tool
| Scenario | Next tool | What you hope to see |
|---|---|---|
| a | `dotnet-trace` (sampling) → flame graph | the hot method (this is Lab L08-02) |
| b | `dotnet-counters` LOH size + `dotnet-gcdump`/allocation trace | who allocates the large arrays; then pool/reuse them (Lab 2) |
| c | `dotnet-stack`/dump `clrstack` on pool threads | many threads in `Task.Wait`/`GetResult`: find and remove the sync-over-async (Lab 4) |
| d | dump `syncblk`/`threads` | one thread holding the monitor while others wait; shrink the critical section (Lab 4) |

## The habit this practises
Classify by **resource** (USE: utilisation, saturation, errors) first; pick the heavier tool second.

## Reveal
- `a` busy loops on 4 threads 
- `b` allocates 100–300 KB arrays constantly
- `c` bursts of 200 `Task.Run(() => asyncCall().Result)` with the pool pinned to 4 threads
- `d` 16 threads taking one lock and sleeping 2 ms inside it
- `e` 200 async loops awaiting `Task.Delay`.
