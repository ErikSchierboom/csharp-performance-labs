# L09-05 - Import endpoint

*Special Delivery*

## Symptom
A partner posts order batches to `POST /import`: 1,000 lines per batch, ~170 KB of JSON with full product details. The import only needs each line's id and quantity, and the endpoint's model declares only those. 1,200 batches from 24 users take **~480 ms** with a p99 of **~25 ms**, allocate **~1.1 GB** (about 900 KB per request, over five times the body), and trigger **~35 gen2 collections** per run. The handler is three lines: read the body, parse it, sum the quantities.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 340 ref-ms |
| Median allocated | 250 MB |
| Median gen2 collections | ≤ 2 |
| Median p99 latency | 15 ref-ms |

> This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.
