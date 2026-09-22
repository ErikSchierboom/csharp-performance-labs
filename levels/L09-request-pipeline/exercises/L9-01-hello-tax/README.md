# L9 · Hello tax (what does a free endpoint cost?)

## Symptom
The simplest possible endpoint (returns `{"message":"hello"}`) is served by a **controller**. At 4,000 requests it uses **more time and memory than the response could possibly justify**. Before you tune real endpoints you need to know what the *floor* is: what does a request cost when the handler does nothing?

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 42 ref-ms |
| Median allocated | 14 MB |
| Median p99 latency | 6 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
This exercise starts an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`PerfLab.Harness.Web.WebRig`). Latency (p50/p99) comes from the client's view of each request.
**Allocation and CPU include the small, constant cost of the load-generating client**, so treat allocation budgets as "server + client". Because both share the machine, results are less exact than the console exercises. `taskset -c 0-7 dotnet run ...` reduces noise.

## Extra credit
Return the object from a minimal API (`Results.Ok(new { message = "hello" })`) instead of a pre-built string. Where does the difference come from?
