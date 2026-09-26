# L09-04 - Price endpoint

*The Price Is Right*

## Symptom
A price endpoint takes a millisecond or more per request and allocates **hundreds of KB per request**, for a lookup in a dictionary. The lookup is trivial; the cost is somewhere else in the request path.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 40 ref-ms |
| Median allocated | 11 MB |
| Median p99 latency | 6 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.
