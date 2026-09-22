# L9 · Request body (buffered into a string)

## Symptom
An import endpoint accepts a ~60 KB JSON body. Each request allocates **far more than the body's size**, and the harness shows **gen2 collections** during a run of only 1,200 small requests. The handler is four lines: read the body, parse it, sum a field.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 1428 ref-ms |
| Median allocated | 450 MB |
| Median gen2 collections | ≤ 2 |
| Median p99 latency | 57 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.

## Extra credit
Call `ctx.Request.EnableBuffering()` in the fix. What does that do to allocation, and when would you need it?
