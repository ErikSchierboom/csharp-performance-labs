# L09-04 - Price endpoint

*The Price Is Right*

## Symptom
`GET /price/{id}?qty=N` quotes a price: look up the product's unit price, multiply, apply a bulk discount. 1,500 requests from 32 users take **~620 ms** with a p99 of **~35 ms**, and allocate **~890 MB**: about 600 KB per request, for an answer of a few bytes. The price list itself is small and never changes while the app runs.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 20 ref-ms |
| Median allocated | 6 MB |
| Median p99 latency | 1 ref-ms |

> This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.
