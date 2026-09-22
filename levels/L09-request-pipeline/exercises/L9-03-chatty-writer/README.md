# L9 · Chatty writer (many small writes)

## Symptom
A report endpoint returns 300 short lines (about 9 KB). It takes far longer per request than its size suggests and **each response goes out as hundreds of tiny network writes**. The handler "streams" the report line by line, and nobody actually needs it to stream.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 106 ref-ms |
| Median allocated | 237 MB |
| Median p99 latency | 8 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.

## Extra credit
Keep the per-line writes but remove the `FlushAsync`. How much of the improvement do you get, and where does the rest come from?
