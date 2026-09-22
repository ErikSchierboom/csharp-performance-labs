# L6-07 · Solution (measured on one machine)

> Numbers below were measured (20-core Linux box, .NET 10, Release, `--cold --runs 8`). Yours will differ; the *shape* is what to check.

## Measured (ms per run)
| Setting | run 1 | runs 2–8 (typical) |
|---|---|---|
| default | 17.0 | 6.5–6.7 |
| `DOTNET_TieredPGO=0` | 14.5 | 7.4–7.9 |
| `DOTNET_TieredCompilation=0` | 14.1 | ~7.1 |
| `DOTNET_TC_QuickJitForLoops=0` | 12.8 | ~7.0 |
| `DOTNET_ReadyToRun=0` | 25.5 | ~13.3 |
| default + `[AggressiveOptimization]` on `Checksum` (this folder's `Workload.cs`) | 16.6 | 6.2–6.5 |

## What it shows
1. **Run 1 is ~2.5× steady state**, mostly compilation and tier-0 execution. Run 2 already looks like run 8 here because the hot loop is promoted quickly (on-stack replacement for the long loop, then tier 1).
2. **Tiering off (`TieredCompilation=0`) or loops skipping tier 0 (`QuickJitForLoops=0`)** improve run 1 by ~15–25% but make the steady state ~8% *slower* than default: you gave up profile-guided optimisation. Turning PGO off alone has the same steady-state cost.
3. **Disabling ReadyToRun** roughly *doubles* every run (in this window): a large share of the time was in framework code (`LINQ`, `Dictionary`) that normally arrives precompiled and is then improved by tiering. This is why publishing your **own** code with ReadyToRun helps first-request latency.
4. **`[MethodImpl(AggressiveOptimization)]` on the hot method made no measurable difference to run 1 here**, because that method is only part of the cost. The lesson: measure before adding an attribute. (It also disables tiering and PGO for that method, so it can *hurt* steady state.)

## Choosing (no free lunch)
| Situation | Prefer |
|---|---|
| Long-running service, latency-sensitive first requests after deploy | ReadyToRun (publish) **plus** a warm-up phase before joining the load balancer; keep tiering and PGO on |
| Short-lived CLI / function | ReadyToRun or Native AOT; accept lower peak |
| Throughput batch job | defaults (tiering + PGO) |
| Micro-hot method that must be fast on call 1 | `AggressiveOptimization`, after measuring |

## Further reading
- [Dynamic PGO design (dotnet/runtime)](https://github.com/dotnet/runtime/blob/main/docs/design/features/DynamicPgo.md) and [OSR details](https://github.com/dotnet/runtime/blob/main/docs/design/features/OsrDetailsAndDebugging.md)
- [Compilation config settings (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/compilation)
- Toub, [Performance Improvements in .NET 10](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/) (JIT and tiering sections)
- Track L13-02 (cold start) applies this to a web service.
