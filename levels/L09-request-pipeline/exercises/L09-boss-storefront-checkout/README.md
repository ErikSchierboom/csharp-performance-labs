# L09-boss - Storefront checkout

*Long Queue at Register One*

## Symptom
A checkout endpoint accepts a ~60 KB basket and answers with a receipt. **Per request it allocates hundreds of KB**, triggers gen2 collections, and takes far longer than the work justifies. A middleware, a DI call, the body read, and the response writing each look reasonable in isolation.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 300 ref-ms |
| Median allocated | 320 MB |
| Median gen2 collections | 0 |
| Median p99 latency | 15 ref-ms |

> This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.

## Final boss fight
This is the **final boss** of its level: a disguised combination of that level's defects, in a different domain, with **no per-defect hints**. Profile it, list what you find, fix one thing at a time, and afterwards write down **which exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.
