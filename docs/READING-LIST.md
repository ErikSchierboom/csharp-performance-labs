# Reading list

Organised by level, matching [ROADMAP.md](../ROADMAP.md). Read **just in time**: attempt the exercise first, then read. Citations are IEEE-style, numbered continuously; the full list is in [References](#references) at the end.

## The core shelf

| # | Reference | Why | Level use |
|---|---|---|---|
| [[1]](#ref1) | Kokosa, Nasarre & Gosse, *Pro .NET Memory Management* | The GC and memory reference. Covers .NET Framework through .NET 8 | 0, 2, 3, 7 |
| [[2]](#ref2) | Toub, "Performance Improvements in .NET" | The best yearly tour of what the runtime does for you and why | all |
| [[3]](#ref3) | Watson, *Writing High-Performance .NET Code* | Broad and practical: measurement, GC, JIT, tools. Older, so check specifics against the runtime version | 0–2, 6 |
| [[4]](#ref4) | Akinshin, *Pro .NET Benchmarking* | How to measure without fooling yourself. By BenchmarkDotNet's project lead | 6, 8 |
| [[5]](#ref5) | Gregg, *Systems Performance* | Methodology, OS-level tools, Linux focus | 7, 8, 13, 14 |
| [[6]](#ref6) | Cleary, *Concurrency in C# Cookbook* | Recipes for async, parallel, Channels, dataflow. Written for C# 8, so newer APIs are missing | 4, 10 |

## Level 0: Foundations
- [[7]](#ref7), [[8]](#ref8): garbage collection fundamentals and performance.
- [[1]](#ref1): chapters 1-4 (memory basics, GC generations).
- [[3]](#ref3): the GC chapter.
- [[9]](#ref9): theory reference, not needed for the labs.
- [[10]](#ref10): a map of the runtime's own docs.
- [[11]](#ref11): a curated index of books, talks and blogs. Use it to find more than this list has.
- [the runtime guide](RUNTIME.md), then [[105]](#ref105) (the settings) and [[106]](#ref106) (the tiered-compilation design), and later [[55]](#ref55), [[56]](#ref56), [[102]](#ref102).
- [[108]](#ref108), [[109]](#ref109): the runtime source that decides between a plain `Memmove` and a write-barrier bulk move (why L0-01's hot frame is `BulkMoveWithWriteBarrierBatch`, not `memmove`).
- [[107]](#ref107): amortised analysis of dynamic arrays, the background to why `List<T>.Insert(0, …)` in a loop is quadratic and `Add` is not (the L0-01 solution's further reading).

## Level 1: Obvious hot spots
- [[12]](#ref12): profiling modes (sampling, tracing, line-by-line, timeline).
- [[13]](#ref13), [[14]](#ref14): why `[GeneratedRegex]` beats `Compiled` (L1-03).
- [[15]](#ref15): `TryParse`, the tester-doer pattern (L1-04).
- [[16]](#ref16): LINQ deferred execution (L1-05).
- [[17]](#ref17), [[18]](#ref18): on reading a call tree as a picture.

## Level 2: Allocations & GC pressure
- [[1]](#ref1): the chapters on allocation, LOH and finalization.
- [[19]](#ref19): the 85,000-byte threshold (L2-03).
- [[20]](#ref20): L2-02, L2-06.
- [[21]](#ref21), [[22]](#ref22): L2-02, L2-03.
- [[23]](#ref23): key line: the default choice is still `Task` (L2-05).
- [[24]](#ref24): how async methods themselves avoid allocating.
- [[25]](#ref25): closure allocations (L2-02), and [[26]](#ref26) his blog generally.
- [[27]](#ref27): for what a closure actually captures.
- [[28]](#ref28): assert memory traffic in a unit test, the same idea as this repo's `MaxAllocatedMB`.
- [[29]](#ref29), [[30]](#ref30): Dispose and finalizers (L2-04).

## Level 3: Leaks & retention
- [[31]](#ref31), [[32]](#ref32): real leak scenarios with walkthroughs (originally WinDbg-based).
- [[33]](#ref33): debug a memory leak tutorial.
- [[1]](#ref1): chapters on GC roots, finalization, weak references.
- [[34]](#ref34): pinned objects (relevant again at Level 7).
- [[35]](#ref35): deep dives into the GC and debugging.
- [[36]](#ref36), [[37]](#ref37): weak references and `ConditionalWeakTable` (L3-05).

## Level 4: Async & concurrency
- [[38]](#ref38): long, worth it. Read after L2-05.
- [[39]](#ref39): debug ThreadPool starvation (L4-01).
- [[40]](#ref40): how queuing makes starvation worse.
- [[6]](#ref6): Channels, `SemaphoreSlim`, throttling.
- [[41]](#ref41): bad/good async patterns from real incidents.
- [[42]](#ref42), [[43]](#ref43): `System.Threading.Channels`, `Parallel.ForEachAsync` (L4-03, L4-05).
- [[44]](#ref44): old, but the reference on locks and memory models.
- [[45]](#ref45): `dotnet-counters`.

## Level 5: Library stack under a single caller
- [[46]](#ref46), [[47]](#ref47): efficient querying, tracking vs. no-tracking (L5-01…03).
- [[48]](#ref48).
- [[49]](#ref49): HttpClient guidelines (L5-04).
- [[50]](#ref50), [[51]](#ref51): JSON source generation, high-performance logging (L5-05, L5-06).
- [[2]](#ref2): the JSON, logging and I/O sections of the yearly performance posts.

## Level 6: Hardware & runtime effects
- [[52]](#ref52): read sections 3 (CPU caches) and 6 (programmer techniques). Old but still the clearest.
- [[53]](#ref53).
- [[4]](#ref4): statistics chapters especially.
- [[54]](#ref54): `MemoryDiagnoser`, `DisassemblyDiagnoser`; note `HardwareCounters` is Windows-only.
- [[55]](#ref55), [[56]](#ref56): dynamic PGO and OSR design docs.
- [[57]](#ref57): the runtime's own internals reference.
- [[58]](#ref58): chapters 5–6 on optimisation and the memory hierarchy.
- [[59]](#ref59), [[60]](#ref60): free PDFs at agner.org.
- [[61]](#ref61): reference only.
- [[2]](#ref2): the JIT sections of the yearly performance posts (guards, devirtualization, bounds-check elimination).

## Level 7: Boss fights
- [[1]](#ref1): the LOH, pinning, POH and GC-mode chapters.
- [[62]](#ref62): posts from the GC architect.
- [[63]](#ref63): garbage collector config settings.
- [[5]](#ref5): methodology chapter (USE, drill-down, workload characterisation).
- [[64]](#ref64): stability patterns and failure stories.

## Level 8: Beyond the IDE
- [[65]](#ref65): the map. Then each tool: [[45]](#ref45), [[66]](#ref66), [[67]](#ref67), [[68]](#ref68).
- [[69]](#ref69), [[70]](#ref70): high CPU usage, containers.
- [[71]](#ref71): opens `dotnet-trace --format Speedscope` output.
- [[17]](#ref17): reading a call tree as a picture, and [[72]](#ref72) the USE method.
- [[73]](#ref73): the four golden signals.
- [[74]](#ref74): the RED method (rate, errors, duration).
- [[75]](#ref75): SLO chapters.
- [[76]](#ref76): optional; Linux tracing beyond .NET.

---

## The ASP.NET Core levels (9–14)

## Level 9: The request pipeline
- [[77]](#ref77): the "avoid blocking" and "minimize large object allocations" sections match L2-03 and L10.
- [[78]](#ref78): memory management and patterns in ASP.NET Core.
- [[79]](#ref79), [[80]](#ref80): ASP.NET Core 8 and 6 performance posts. I did not find a dedicated ASP.NET Core 10 post; check [[81]](#ref81).
- [[82]](#ref82): pipeline, DI, hosting, and [[83]](#ref83) his blog generally.
- [[84]](#ref84): general ASP.NET Core guidance.
- [[85]](#ref85): dependency injection and service-lifetime guidance (L9-04).
- [[86]](#ref86): see what a maximally tuned pipeline can reach, and treat it as an upper bound, not a target.

## Level 10: Async, threads & the pool under load
- [[41]](#ref41), [[87]](#ref87): async and HttpClient guidance.
- [[39]](#ref39): note the `WaitHandleWait` event added in .NET 9 for spotting blocked threads.
- [[40]](#ref40).
- [[38]](#ref38).
- [[88]](#ref88): background tasks with hosted services (L10-06).

## Level 11: Data access under load
- [[46]](#ref46), [[47]](#ref47).
- [[89]](#ref89): DbContext pooling, compiled queries.
- [[90]](#ref90): chapters on storage/indexes and transactions.
- [[91]](#ref91): the clearest treatment of indexes and paging (L11-02, L5-03).
- [[92]](#ref92): Npgsql connection pooling (L11-04).
- [[93]](#ref93): finding N+1 from traces (L11-01).

## Level 12: Caching & outbound calls
- [[94]](#ref94), [[95]](#ref95), [[96]](#ref96): HybridCache adds stampede protection over `MemoryCache`.
- [[97]](#ref97): rate limiting middleware.
- [[49]](#ref49).
- [[98]](#ref98): resilient HTTP apps (L12-05).
- [[64]](#ref64): timeouts, circuit breakers, bulkheads, back-pressure.
- [[99]](#ref99): timeouts, retries and backoff with jitter.

## Level 13: Hosting, runtime config & deployment
- [[63]](#ref63).
- [[100]](#ref100): DATAS is on by default from .NET 9; expect different Server-GC memory behaviour after upgrading.
- [[70]](#ref70).
- [[101]](#ref101): `http.server.request.duration`, Kestrel connection metrics.
- [[102]](#ref102), [[103]](#ref103): ReadyToRun, Kestrel.
- [[5]](#ref5): CPU and cgroup sections.

## Level 14: Production diagnosis & capstones
- [[73]](#ref73) and [[75]](#ref75): SLO chapters.
- [[104]](#ref104): essential before you trust any load-test percentile.
- [[5]](#ref5): methodology.
- [[93]](#ref93).
- Load tools worth knowing: `bombardier`, `wrk`/`wrk2`, `k6`, `NBomber`, `oha`
- [[73]](#ref73): the postmortem chapters, on writing blameless post-mortems.

---

## References

<a id="ref1"></a>[1] K. Kokosa, C. Nasarre, and K. Gosse, *Pro .NET Memory Management: For Better Code, Performance, and Scalability*, 2nd ed. Apress, 2024. [Online]. Available: [https://prodotnetmemory.com/](https://prodotnetmemory.com/)

<a id="ref2"></a>[2] S. Toub, "Performance Improvements in .NET," Microsoft .NET Blog, annual series. [Online]. Available: [https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/) (also the [performance category](https://devblogs.microsoft.com/dotnet/category/performance/))

<a id="ref3"></a>[3] B. Watson, *Writing High-Performance .NET Code*, 2nd ed. 2018. [Online]. Available: [https://www.writinghighperf.net/](https://www.writinghighperf.net/)

<a id="ref4"></a>[4] A. Akinshin, *Pro .NET Benchmarking*. Apress, 2019. [Online]. Available: [https://www.apress.com/us/book/9781484249406](https://www.apress.com/us/book/9781484249406)

<a id="ref5"></a>[5] B. Gregg, *Systems Performance*, 2nd ed. Addison-Wesley, 2020. [Online]. Available: [https://www.brendangregg.com/systems-performance-2nd-edition-book.html](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)

<a id="ref6"></a>[6] S. Cleary, *Concurrency in C# Cookbook*, 2nd ed. O'Reilly, 2019. [Online]. Available: [https://stephencleary.com/book/](https://stephencleary.com/book/)

<a id="ref7"></a>[7] Microsoft, "Fundamentals of garbage collection," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)

<a id="ref8"></a>[8] Microsoft, "Garbage collection and performance," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/performance](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/performance)

<a id="ref9"></a>[9] R. Jones, A. Hosking, and J. E. B. Moss, *The Garbage Collection Handbook: The Art of Automatic Memory Management*, 2nd ed. CRC Press, 2023. [Online]. Available: [https://www.routledge.com/The-Garbage-Collection-Handbook-The-Art-of-Automatic-Memory-Management/Jones-Hosking-Moss/p/book/9781032231785](https://www.routledge.com/The-Garbage-Collection-Handbook-The-Art-of-Automatic-Memory-Management/Jones-Hosking-Moss/p/book/9781032231785)

<a id="ref10"></a>[10] M. Warren, "Resources for learning about .NET internals," personal blog, Jan. 2018. [Online]. Available: [https://mattwarren.org/2018/01/22/Resources-for-Learning-about-.NET-Internals/](https://mattwarren.org/2018/01/22/Resources-for-Learning-about-.NET-Internals/)

<a id="ref11"></a>[11] A. Sitnik, "awesome-dot-net-performance," GitHub. [Online]. Available: [https://github.com/adamsitnik/awesome-dot-net-performance](https://github.com/adamsitnik/awesome-dot-net-performance)

<a id="ref12"></a>[12] JetBrains, "Basics. Profiling Types," dotTrace Documentation. [Online]. Available: [https://www.jetbrains.com/help/profiler/Basic_Concepts.html](https://www.jetbrains.com/help/profiler/Basic_Concepts.html)

<a id="ref13"></a>[13] Microsoft, "Best practices for regular expressions in .NET," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-regex](https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-regex)

<a id="ref14"></a>[14] Microsoft, ".NET regular expression source generators," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators)

<a id="ref15"></a>[15] Microsoft, "Best practices for exceptions," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

<a id="ref16"></a>[16] Microsoft, "LINQ deferred execution (lazy evaluation)," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/linq/deferred-execution-lazy-evaluation](https://learn.microsoft.com/en-us/dotnet/standard/linq/deferred-execution-lazy-evaluation)

<a id="ref17"></a>[17] B. Gregg, "The Flame Graph," *ACM Queue*, vol. 14, no. 2, 2016. [Online]. Available: [https://queue.acm.org/detail.cfm?id=N2927301](https://queue.acm.org/detail.cfm?id=N2927301)

<a id="ref18"></a>[18] B. Gregg, "Flame Graphs," personal site. [Online]. Available: [https://www.brendangregg.com/flamegraphs.html](https://www.brendangregg.com/flamegraphs.html)

<a id="ref19"></a>[19] Microsoft, "Large object heap," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap)

<a id="ref20"></a>[20] S. Toub, "All About Span: Exploring a New .NET Mainstay," *MSDN Magazine*, Jan. 2018. [Online]. Available: [https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay](https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay)

<a id="ref21"></a>[21] A. Sitnik, "Span," personal blog. [Online]. Available: [https://adamsitnik.com/Span/](https://adamsitnik.com/Span/)

<a id="ref22"></a>[22] A. Sitnik, "Pooling large arrays with ArrayPool," personal blog. [Online]. Available: [https://adamsitnik.com/Array-Pool/](https://adamsitnik.com/Array-Pool/)

<a id="ref23"></a>[23] S. Toub, "Understanding the Whys, Whats, and Whens of ValueTask," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)

<a id="ref24"></a>[24] S. Toub, "Async ValueTask Pooling in .NET 5," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/async-valuetask-pooling-in-net-5/](https://devblogs.microsoft.com/dotnet/async-valuetask-pooling-in-net-5/)

<a id="ref25"></a>[25] S. Teplyakov, "Unusual ways of boosting up app performance: lambdas and LINQs," JetBrains .NET Blog, 2014. [Online]. Available: [https://blog.jetbrains.com/dotnet/2014/07/24/unusual-ways-of-boosting-up-app-performance-lambdas-and-linqs/](https://blog.jetbrains.com/dotnet/2014/07/24/unusual-ways-of-boosting-up-app-performance-lambdas-and-linqs/)

<a id="ref26"></a>[26] S. Teplyakov, "Dissecting the Code," personal blog. [Online]. Available: [https://sergeyteplyakov.github.io/Blog/](https://sergeyteplyakov.github.io/Blog/)

<a id="ref27"></a>[27] E. Lippert, "Closing over the loop variable considered harmful," *Fabulous adventures in coding*, Nov. 2009. [Online]. Available: [https://ericlippert.com/2009/11/12/closing-over-the-loop-variable-considered-harmful-part-one/](https://ericlippert.com/2009/11/12/closing-over-the-loop-variable-considered-harmful-part-one/)

<a id="ref28"></a>[28] JetBrains, "dotMemory Unit," JetBrains Help. [Online]. Available: [https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html](https://www.jetbrains.com/help/dotmemory-unit/Get_Started.html)

<a id="ref29"></a>[29] Microsoft, "Implement a Dispose method," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose)

<a id="ref30"></a>[30] Microsoft, "Finalizers," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers)

<a id="ref31"></a>[31] T. Ferrandez, "Buggy Bits," GitHub. [Online]. Available: [https://github.com/TessFerrandez/BuggyBits](https://github.com/TessFerrandez/BuggyBits)

<a id="ref32"></a>[32] T. Ferrandez, blog archive. [Online]. Available: [https://www.tessferrandez.com/posts/](https://www.tessferrandez.com/posts/)

<a id="ref33"></a>[33] Microsoft, "Debug a memory leak," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-memory-leak)

<a id="ref34"></a>[34] Microsoft, "Internals of the POH," .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/internals-of-the-poh/](https://devblogs.microsoft.com/dotnet/internals-of-the-poh/)

<a id="ref35"></a>[35] K. Gosse, "minidump.net," personal blog. [Online]. Available: [https://minidump.net/](https://minidump.net/)

<a id="ref36"></a>[36] Microsoft, "Weak references," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/weak-references](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/weak-references)

<a id="ref37"></a>[37] Microsoft, "ConditionalWeakTable<TKey,TValue> API docs," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.conditionalweaktable-2](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.conditionalweaktable-2)

<a id="ref38"></a>[38] S. Toub, "How Async/Await Really Works in C#," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/how-async-await-really-works/](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)

<a id="ref39"></a>[39] Microsoft, "Debug a ThreadPool starvation issue," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)

<a id="ref40"></a>[40] K. Gosse, ".NET ThreadPool starvation, and how queuing makes it worse," Criteo Engineering, Medium. [Online]. Available: [https://medium.com/criteo-engineering/net-threadpool-starvation-and-how-queuing-makes-it-worse-512c8d570527](https://medium.com/criteo-engineering/net-threadpool-starvation-and-how-queuing-makes-it-worse-512c8d570527)

<a id="ref41"></a>[41] D. Fowler, "AsyncGuidance.md," AspNetCoreDiagnosticScenarios, GitHub. [Online]. Available: [https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)

<a id="ref42"></a>[42] Microsoft, "System.Threading.Channels API docs," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels](https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels)

<a id="ref43"></a>[43] Microsoft, "Parallel.ForEachAsync API docs," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreachasync](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreachasync)

<a id="ref44"></a>[44] J. Duffy, *Concurrent Programming on Windows*. Addison-Wesley, 2008. [Online]. Available: [https://www.oreilly.com/library/view/concurrent-programming-on/9780321434821](https://www.oreilly.com/library/view/concurrent-programming-on/9780321434821)

<a id="ref45"></a>[45] Microsoft, "dotnet-counters," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters)

<a id="ref46"></a>[46] Microsoft, "Efficient querying," EF Core docs, Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)

<a id="ref47"></a>[47] Microsoft, "Tracking vs. no-tracking queries," EF Core docs, Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/ef/core/querying/tracking](https://learn.microsoft.com/en-us/ef/core/querying/tracking)

<a id="ref48"></a>[48] J. P. Smith, *Entity Framework Core in Action*, 2nd ed. Manning. [Online]. Available: [https://www.manning.com/books/entity-framework-core-in-action-second-edition](https://www.manning.com/books/entity-framework-core-in-action-second-edition)

<a id="ref49"></a>[49] Microsoft, "HttpClient guidelines for .NET," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)

<a id="ref50"></a>[50] Microsoft, "How to use source generation in System.Text.Json," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)

<a id="ref51"></a>[51] Microsoft, "High-performance logging," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging)

<a id="ref52"></a>[52] U. Drepper, "What Every Programmer Should Know About Memory," 2007. [Online]. Available: [https://www.akkadia.org/drepper/cpumemory.pdf](https://www.akkadia.org/drepper/cpumemory.pdf)

<a id="ref53"></a>[53] D. Bakhvalov, *Performance Analysis and Tuning on Modern CPUs*. [Online]. Available: [https://book.easyperf.net/perf_book](https://book.easyperf.net/perf_book)

<a id="ref54"></a>[54] BenchmarkDotNet, "Diagnosers," BenchmarkDotNet docs. [Online]. Available: [https://benchmarkdotnet.org/articles/configs/diagnosers.html](https://benchmarkdotnet.org/articles/configs/diagnosers.html)

<a id="ref55"></a>[55] .NET Foundation, "Dynamic PGO design," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/docs/design/features/DynamicPgo.md](https://github.com/dotnet/runtime/blob/main/docs/design/features/DynamicPgo.md)

<a id="ref56"></a>[56] .NET Foundation, "OSR details and debugging," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/docs/design/features/OsrDetailsAndDebugging.md](https://github.com/dotnet/runtime/blob/main/docs/design/features/OsrDetailsAndDebugging.md)

<a id="ref57"></a>[57] .NET Foundation, "Book of the Runtime," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/README.md](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/README.md)

<a id="ref58"></a>[58] R. E. Bryant and D. R. O'Hallaron, *Computer Systems: A Programmer's Perspective*, 3rd ed. Pearson, 2016. [Online]. Available: [https://csapp.cs.cmu.edu/](https://csapp.cs.cmu.edu/)

<a id="ref59"></a>[59] A. Fog, "Optimizing software in C++: An optimization guide for Windows, Linux and Mac platforms," agner.org. [Online]. Available: [https://www.agner.org/optimize/optimizing_cpp.pdf](https://www.agner.org/optimize/optimizing_cpp.pdf)

<a id="ref60"></a>[60] A. Fog, "The microarchitecture of Intel, AMD and VIA CPUs: An optimization guide for assembly programmers and compiler makers," agner.org. [Online]. Available: [https://www.agner.org/optimize/microarchitecture.pdf](https://www.agner.org/optimize/microarchitecture.pdf)

<a id="ref61"></a>[61] J. L. Hennessy and D. A. Patterson, *Computer Architecture: A Quantitative Approach*, 6th ed. Morgan Kaufmann, 2017. [Online]. Available: [https://shop.elsevier.com/books/computer-architecture/hennessy/978-0-12-811905-1](https://shop.elsevier.com/books/computer-architecture/hennessy/978-0-12-811905-1)

<a id="ref62"></a>[62] M. Stephens, posts on the .NET Blog, Microsoft. [Online]. Available: [https://devblogs.microsoft.com/dotnet/author/maoni/](https://devblogs.microsoft.com/dotnet/author/maoni/)

<a id="ref63"></a>[63] Microsoft, "Garbage collector config settings," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector)

<a id="ref64"></a>[64] M. T. Nygard, *Release It!: Design and Deploy Production-Ready Software*, 2nd ed. Pragmatic Bookshelf, 2018. [Online]. Available: [https://www.oreilly.com/library/view/release-it-2nd/9781680504552/](https://www.oreilly.com/library/view/release-it-2nd/9781680504552/)

<a id="ref65"></a>[65] Microsoft, ".NET diagnostics tools overview," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/)

<a id="ref66"></a>[66] Microsoft, "dotnet-gcdump," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-gcdump](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-gcdump)

<a id="ref67"></a>[67] Microsoft, "dotnet-dump," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-dump](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-dump)

<a id="ref68"></a>[68] Microsoft, "Dumps," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dumps](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dumps)

<a id="ref69"></a>[69] Microsoft, "Debug a high CPU usage issue," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-highcpu](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-highcpu)

<a id="ref70"></a>[70] Microsoft, "Collect diagnostics in Linux containers," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/diagnostics-in-containers](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/diagnostics-in-containers)

<a id="ref71"></a>[71] "speedscope." [Online]. Available: [https://www.speedscope.app](https://www.speedscope.app)

<a id="ref72"></a>[72] B. Gregg, "The Utilization Saturation and Errors (USE) Method," personal site. [Online]. Available: [https://www.brendangregg.com](https://www.brendangregg.com)

<a id="ref73"></a>[73] Google, *Site Reliability Engineering*. O'Reilly, 2016. [Online]. Available: [https://sre.google/sre-book/monitoring-distributed-systems/](https://sre.google/sre-book/monitoring-distributed-systems/) (also the postmortem chapters)

<a id="ref74"></a>[74] T. Wilkie, "The RED Method: How to Instrument Your Services," Grafana Labs blog, 2018. [Online]. Available: [https://grafana.com/blog/the-red-method-how-to-instrument-your-services/](https://grafana.com/blog/the-red-method-how-to-instrument-your-services/)

<a id="ref75"></a>[75] B. Beyer, N. R. Murphy, D. K. Rensin, K. Kawahara, and S. Thorne, Eds., *The Site Reliability Workbook: Practical Ways to Implement SRE*. O'Reilly, 2018. [Online]. Available: [https://sre.google/workbook/table-of-contents/](https://sre.google/workbook/table-of-contents/)

<a id="ref76"></a>[76] B. Gregg, *BPF Performance Tools: Linux System and Application Observability*. Addison-Wesley, 2019. [Online]. Available: [https://www.brendangregg.com/bpf-performance-tools-book.html](https://www.brendangregg.com/bpf-performance-tools-book.html)

<a id="ref77"></a>[77] Microsoft, "ASP.NET Core Best Practices," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)

<a id="ref78"></a>[78] Microsoft, "Memory management and patterns in ASP.NET Core," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/performance/memory?view=aspnetcore-10.0)

<a id="ref79"></a>[79] B. Conroy, "Performance Improvements in ASP.NET Core 8," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-8/)

<a id="ref80"></a>[80] B. Conroy, "Performance improvements in ASP.NET Core 6," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-6/](https://devblogs.microsoft.com/dotnet/performance-improvements-in-aspnet-core-6/)

<a id="ref81"></a>[81] Microsoft, "ASP.NET Core tag," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/tag/asp-net-core/](https://devblogs.microsoft.com/dotnet/tag/asp-net-core/)

<a id="ref82"></a>[82] A. Lock, *ASP.NET Core in Action*, 3rd ed. Manning, 2023. [Online]. Available: [https://www.manning.com/books/asp-net-core-in-action-third-edition](https://www.manning.com/books/asp-net-core-in-action-third-edition)

<a id="ref83"></a>[83] A. Lock, "andrewlock.net," personal blog. [Online]. Available: [https://andrewlock.net](https://andrewlock.net)

<a id="ref84"></a>[84] D. Fowler, "AspNetCoreGuidance.md," AspNetCoreDiagnosticScenarios, GitHub. [Online]. Available: [https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AspNetCoreGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AspNetCoreGuidance.md)

<a id="ref85"></a>[85] Microsoft, "Dependency injection in ASP.NET Core," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

<a id="ref86"></a>[86] TechEmpower, "Framework Benchmarks." [Online]. Available: [https://www.techempower.com/benchmarks/](https://www.techempower.com/benchmarks/)

<a id="ref87"></a>[87] D. Fowler, "HttpClientGuidance.md," AspNetCoreDiagnosticScenarios, GitHub. [Online]. Available: [https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/HttpClientGuidance.md](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/HttpClientGuidance.md)

<a id="ref88"></a>[88] Microsoft, "Background tasks with hosted services in ASP.NET Core," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)

<a id="ref89"></a>[89] Microsoft, "Advanced performance topics," EF Core docs, Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics](https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics)

<a id="ref90"></a>[90] M. Kleppmann, *Designing Data-Intensive Applications: The Big Ideas Behind Reliable, Scalable, and Maintainable Systems*. O'Reilly, 2017. [Online]. Available: [https://dataintensive.net/](https://dataintensive.net/)

<a id="ref91"></a>[91] M. Winand, *SQL Performance Explained*. [Online]. Available: [https://use-the-index-luke.com/](https://use-the-index-luke.com/)

<a id="ref92"></a>[92] Npgsql, "Basic Usage," Npgsql Documentation. [Online]. Available: [https://www.npgsql.org/doc/basic-usage.html](https://www.npgsql.org/doc/basic-usage.html)

<a id="ref93"></a>[93] Microsoft, ".NET Observability with OpenTelemetry," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)

<a id="ref94"></a>[94] Microsoft, "Overview of caching in ASP.NET Core," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-10.0)

<a id="ref95"></a>[95] Microsoft, "Output caching middleware," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-10.0)

<a id="ref96"></a>[96] Microsoft, "HybridCache," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)

<a id="ref97"></a>[97] Microsoft, "Rate limiting middleware," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0)

<a id="ref98"></a>[98] Microsoft, "Build resilient HTTP apps: Key development patterns," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience](https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience)

<a id="ref99"></a>[99] M. Brooker, "Timeouts, retries and backoff with jitter," Amazon Builders' Library. [Online]. Available: [https://aws.amazon.com/builders-library/timeouts-retries-and-backoff-with-jitter](https://aws.amazon.com/builders-library/timeouts-retries-and-backoff-with-jitter)

<a id="ref100"></a>[100] M. Stephens, "Preparing for the .NET 10 GC (DATAS)," Microsoft .NET Blog. [Online]. Available: [https://devblogs.microsoft.com/dotnet/preparing-for-dotnet-10-gc/](https://devblogs.microsoft.com/dotnet/preparing-for-dotnet-10-gc/)

<a id="ref101"></a>[101] Microsoft, "ASP.NET Core built-in metrics," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/metrics/built-in?view=aspnetcore-10.0](https://learn.microsoft.com/en-us/aspnet/core/metrics/built-in?view=aspnetcore-10.0)

<a id="ref102"></a>[102] Microsoft, "ReadyToRun deployment overview," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/deploying/ready-to-run](https://learn.microsoft.com/en-us/dotnet/core/deploying/ready-to-run)

<a id="ref103"></a>[103] Microsoft, "Kestrel web server implementation in ASP.NET Core," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel)

<a id="ref104"></a>[104] G. Tene, "How NOT to Measure Latency," presented at QCon, 2013. [Online]. Available: [https://www.infoq.com/presentations/latency-response-time](https://www.infoq.com/presentations/latency-response-time)

<a id="ref105"></a>[105] Microsoft, "Compilation config settings," Microsoft Learn. [Online]. Available: [https://learn.microsoft.com/en-us/dotnet/core/runtime-config/compilation](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/compilation)

<a id="ref106"></a>[106] .NET Foundation, "Tiered compilation," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/docs/design/features/tiered-compilation.md](https://github.com/dotnet/runtime/blob/main/docs/design/features/tiered-compilation.md)

<a id="ref107"></a>[107] T. H. Cormen, C. E. Leiserson, R. L. Rivest, and C. Stein, *Introduction to Algorithms*, 4th ed. Cambridge, MA, USA: MIT Press, 2022. ISBN 978-0-262-04630-5.

<a id="ref108"></a>[108] .NET Foundation, "Buffer.cs," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Buffer.cs](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Buffer.cs)

<a id="ref109"></a>[109] .NET Foundation, "Array.cs," dotnet/runtime, GitHub. [Online]. Available: [https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Array.cs](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Array.cs)
