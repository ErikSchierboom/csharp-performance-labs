# Questions
Predict each *before* running.

| Limit / setting | Prediction: survives? throughput vs. baseline? | Observed exit code | Observed `limit=` in output | Observed throughput (allocations) |
|---|---|---|---|---|
| none | | | | |
| 260M | | | | |
| 220M | | | | |
| 200M | | | | |
| 220M + `GCHeapHardLimit=200MB` | | | | |
| 220M + `GCConserveMemory=9` | | | | |

1. What memory limit does the GC read from the container, and what fraction of it does it allow the *managed heap* by default? Where did you see it?
2. Why does throughput drop at 220M even though the process still fits? What is the GC doing more of? (Look at the gen0/gen1/gen2 counts.)
3. Why does the process **crash** at 200M with "Out of memory" rather than being killed by the kernel?
4. Why does an explicit `GCHeapHardLimit` (200 MB) do better than the default at 220M? What is the risk of setting it that high in a real container?
5. What would you configure for a real service with a 150 MB live set and a 256 MB limit? What would you measure to confirm?
