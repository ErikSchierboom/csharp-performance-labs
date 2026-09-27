# Roadmap

If you just want to get going, the [top-level README](README.md) has the quick start. This file is the detailed map: what each level teaches, what's built versus still just a sketch, and why the order is what it is.

One sequence of **fifteen levels (0–14)** that share one method (measure → profile → hypothesise → change one thing → re-measure):

- **Levels 0–8, the .NET runtime:** CPU, allocation, GC, memory, concurrency, hardware, production tooling.
- **Levels 9–14, ASP.NET Core:** the same skills under *concurrent load*, where new failure modes appear (thread-pool starvation, pool exhaustion, stampedes, retry storms, connection churn).

Books, articles and docs for every level: [docs/READING-LIST.md](docs/READING-LIST.md). All projects target **.NET 10**. Every level lives in its own folder under [`levels/`](levels/) (`L00-start-here` … `L14-capstones`), with its own README.

**Status legend:**

- ✅ built and verified (slow version fails its budgets, fixed version passes, same checksum)
- 📐 designed
- 🛠️ being built
- 💡 idea

| Level | Theme                                            | Profiling skill                               | Exercises | Final boss                    | Status |
|-------|--------------------------------------------------|-----------------------------------------------|-----------|-------------------------------|--------|
| 0     | Start here: worked example + foundations reading | sampling                                      | 1         | n/a                           | ✅     |
| 1     | Obvious hot spots                                | sampling, tracing                             | 7         | `L01-boss-order-ledger`        | ✅     |
| 2     | Allocations & GC pressure                        | allocation profiling                          | 9         | `L02-boss-shipment-manifest`   | ✅     |
| 3     | Leaks & retention                                | heap snapshots, dominators                    | 6         | `L03-boss-session-gateway`     | ✅     |
| 4     | Async & concurrency                              | timeline view, `dotnet-counters`              | 7         | `L04-boss-notification-hub`    | ✅     |
| 5     | Library stack, single caller                     | SQL/HTTP/IO subsystem views                   | 8         | `L05-boss-order-report`        | ✅     |
| 6     | Hardware & runtime effects                       | `perf stat`, JIT disassembly, BenchmarkDotNet | 9         | `L06-boss-sensor-grid`         | ✅     |
| 7     | Boss fights                                      | everything                                    | 4         | (the whole level)             | ✅     |
| 8     | Beyond the IDE (labs)                            | `dotnet-counters/trace/gcdump/dump`, cgroups  | 5 labs    | (L08-05 ties it together)      | ✅     |
| 9     | ASP.NET: the request pipeline                    | load driver, allocation profiling             | 5         | `L09-boss-storefront-checkout` | ✅     |
| 10    | ASP.NET: async, threads & the pool under load    | counters, timeline view                       | 6         | `L10-boss-async-quotes`       | ✅     |
| 11    | ASP.NET: data access under load                  | EF logging, SQL counts                        | 4         | `L11-boss-orders-service`     | 🛠️     |
| 12    | ASP.NET: caching & outbound calls                | counters                                      | 5         | `L12-boss-catalog-service`    | 🛠️     |
| 13    | ASP.NET: hosting, runtime config, deployment     | keep-alive, logging, GC settings              | 2         | n/a                           | 🛠️     |
| 14    | ASP.NET: production capstones                    | everything                                    | 2         | (the whole level)             | 🛠️     |

*"Profiling skill" names the capability you need, not a specific product; see [docs/PROFILING-GUIDE.md](docs/PROFILING-GUIDE.md) for which tools (Rider, Visual Studio, the free CLI tools, `perf`) give you each one.*
## Specializations

Optional side tracks that branch off the core levels, so the core stays at fifteen levels. Each track has its own numbering, prerequisite and boss fight, and none is needed to finish Levels 0–14. Planned layout: `specializations/<track>/{exercises,solutions}`.

| Track                          | Prerequisite | Topics                                                            | Status |
|--------------------------------|--------------|-------------------------------------------------------------------|--------|
| Databases: RavenDB             | L4, L11      | sessions, N+1, indexes, projections, bulk insert                  | 💡     |
| Serialization & wire formats   | L2, L5       | source generators, streaming readers, Protobuf/MessagePack        | 💡     |
| Observability                  | L9, L13      | OpenTelemetry overhead, log volume, metric cardinality, sampling  | 💡     |
| Messaging & background work    | L4, L10      | `Channel<T>`, backpressure, batching, consumer lag, poison messages | 💡     |
| Startup, AOT & trimming        | L6, L13      | cold start, tiered JIT, ReadyToRun, NativeAOT                     | 💡     |
| Systems design patterns        | L4, L12      | rate limiting, circuit breaker, bulkhead, idempotency, distributed locks, consistent hashing, replication/quorum | 💡     |
| gRPC & HTTP/2-3                | L9           | streaming, multiplexing, header compression, connection/stream limits | 💡     |

**How to work through it:** in order, 0 to 14. Each level ends with a **final boss fight**: a disguised combination of that level's defects with no per-defect hints (Levels 7 and 14 *are* boss levels). Levels 9–14 assume Levels 1–4; if you want ASP.NET sooner you can take Levels 9–10 right after Level 4 and Levels 11–12 after Level 5. Level 6 is independent of Levels 3–5. Each level has a **mastery checkpoint**: something to do *without notes* before moving on.