# L09-02 - Audit middleware

*Toll Booth on Every Request*

## Symptom
A small audit middleware runs on every request: it tags the request, logs it, and adds a header. The log level means **nothing is ever written**. Yet per request it allocates **several KB** and burns CPU, and the endpoint behind it is a one-liner. The cost is all in the 15 lines of middleware.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 30 ref-ms |
| Median allocated | 17 MB |
| Median p99 latency | 1 ref-ms |

> This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.
