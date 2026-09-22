# L10 · Async lock around a cacheable call

## Symptom
A currency-conversion endpoint fetches the rate from a remote service (10 ms). A `SemaphoreSlim(1)` guards the fetch "so we don't hammer the API". With 50 users the **whole endpoint is limited to ~100 requests per second and the p99 is a full second or more**, even though there are only four distinct currencies and the rates barely change.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 12 ref-ms |
| Median allocated | 3 MB |
| Median p99 latency | 6 ref-ms |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** and drives it with virtual users (`WebRig`). The thread pool's *minimum* thread count is pinned to 4 before each run (`Workload.Reset`, scaffolding, not the fix) so the effect doesn't depend on your core count. Allocation and CPU include the small constant client cost.

## Extra credit
What if the rate can differ per *user* (many keys)? What would you use to bound the cache size?
