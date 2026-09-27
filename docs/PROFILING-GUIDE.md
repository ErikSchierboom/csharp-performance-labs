# Profiling guide

A profiler can't tell you what's wrong. It can only tell you where time and memory actually went, which is usually surprising enough on its own. This is the cheat sheet for turning that into an answer.

Everything below is written in terms of **modes** (sampling, tracing, timeline, allocations, snapshots), not one specific product, because any real profiler gives you these in some form. Concrete steps are included for Rider, Visual Studio and the free CLI tools; use whichever you've got.

## The loop
1. **Measure** a baseline (harness output): a real number to beat, not a vague sense that it's slow.
2. **Profile** with the right mode: the table below maps symptom to mode; guessing wastes a run.
3. **Hypothesise**: one sentence, written down, *before* you touch any code: what's wrong, and why you think so.
4. **Experiment**: change exactly one thing. Two changes at once means you won't know which one mattered.
5. **Re-measure**: same harness, same machine otherwise idle, no shortcuts.
6. Explain *why* the number moved. If you can't, you got lucky, and luck doesn't generalise to the next bug.

## Pick the mode for the symptom
| Symptom | Mode you need | Why | Where to find it |
|---|---|---|---|
| High CPU, "it's slow" | **Sampling** | Low overhead; shows where CPU time goes | Rider dotTrace, VS CPU Usage, `dotnet-trace` |
| "Called too often?" / exact counts | **Tracing** | Exact call counts; distorts timings. Use for counts, then confirm with sampling | Rider dotTrace tracing mode |
| Which *line* in a hot method | **Line-by-line** | Precise but heavy; only after sampling narrowed it down | Rider dotTrace line-by-line, VS per-line hit counts |
| Low CPU but slow / waiting / threads | **Timeline** | Shows thread states, blocking, GC pauses, I/O over time | Rider dotTrace timeline, VS Concurrency Visualizer, `dotnet-trace` → speedscope |
| Lots of GCs, high allocation rate | **Allocations** | Allocation call stacks | Rider dotMemory, VS .NET Object Allocation, `dotnet-counters` alloc-rate |
| Memory keeps growing / "leak" | **Snapshots** (diff) + dominators | Who retains what | Rider dotMemory compare, VS Memory Usage snapshots, `dotnet-gcdump` |
| Live, in production | **Low-overhead CLI** | No IDE, low overhead | `dotnet-counters`, `dotnet-trace`, `dotnet-gcdump` |

Every profiler, whatever it's called, is really offering some subset of these. Knowing what each one is actually doing under the hood is what lets you pick correctly instead of by habit.

- **Sampling.** Every millisecond or so, the profiler interrupts every thread and writes down its current call stack. Do that a few thousand times and the frames that show up most often are the ones actually costing you CPU time. It's statistics, not exact measurement (a very fast method that happens to run in the gaps between samples can be under-counted), but the overhead is low enough that the program still runs at close to normal speed. This is your default first move for "why is this slow."
- **Tracing (instrumentation).** The profiler rewrites the code, or hooks the runtime, to record an event on *every* method entry and exit. You get exact call counts and exact per-call timing, no statistics involved, but the instrumentation itself takes time, especially on small, frequently-called methods, so the timings it reports can be inflated relative to reality. Good for "how many times is this actually called," bad for trusting the absolute milliseconds it shows you.
- **Line-by-line.** The same idea as tracing, but recording a hit count and time per *source line* instead of per method. The most precise of the three, and the heaviest: only reach for it after sampling has already told you which method to look inside.
- **Timeline.** A chronological record of what every thread was doing over wall-clock time: running, waiting on a lock, blocked on I/O, paused by the GC. Sampling answers "what is the CPU doing"; timeline answers "why is the CPU sitting idle." Reach for it whenever something's slow but CPU usage looks low or normal: that's the signature of waiting, not computing, and sampling alone will mislead you.
- **Allocations.** Hooks into the allocator to record every object allocation together with the call stack that caused it (and usually which GC generation each object ends up surviving into). This is how you find *who* is generating the garbage, as opposed to timeline/sampling telling you *that* garbage collection is happening.
- **Snapshots (heap diff).** Pauses the process and walks the entire live object graph: a complete inventory of every object and what's holding a reference to it. Take one snapshot, run some work, take another, and diff them: whatever's left over is either legitimately still needed, or a leak. This is the only mode of the six built for answering "why does memory keep growing" rather than "why is this slow."

Rough overhead ordering, cheapest to most invasive: **sampling ≈ allocations ≈ timeline < tracing < line-by-line.** Start cheap, and only reach for something heavier once a cheaper mode has narrowed down where to look.

## Reading a call tree
- **Own/self time** = time in that method's own code. **Total/cumulative** = including callees.
- Sort by self time to find the *leaf* doing the work; then walk **up** to the first frame that's *your code*.
- The biggest total-time frame is often just the entry point. It tells you nothing.
- In a multi-threaded process the root total is the sum over threads, so it is larger than the run's wall-clock time (and check the unit: `43,240 ms` is 43 s). Look at the thread you care about, usually the one running `Main`.
- A method that "looks expensive" and a method that *is* expensive are different things. Trust the numbers.
- Ask "how many times?" as well as "how long?". 1 µs x 100 million is a problem; 10 ms x 1 isn't.

## Traps
- **Debug builds** (JIT optimizer off) and an **attached debugger**: the numbers are meaningless. Profile Release, via "Profile", not "Debug". Note that the **Profile** launch profile only supplies the arguments (`--profile --seconds 15`); it does not select the build configuration, so an IDE run is Debug until you switch the solution configuration to Release. The harness prints `Debug build detected` when that happens.
- **JIT warm-up** and tiered compilation: the first run is slower. The harness warms up; a real profile of a cold process shows startup, which is a different problem. ([RUNTIME.md](RUNTIME.md) explains the stages.)
- **One run isn't data.** Look at several; watch for GC-induced variance.
- **Profiler overhead** changes behaviour (tracing especially). Confirm a finding with a second, lighter method.
- **Don't optimise what the profile doesn't show.** And re-profile after each fix: the picture changes.

## Workflows
### Rider workflow
1. Run configuration: the **Profile** launch profile (args `--profile --seconds 15`). Switch the solution configuration to **Release** first: the launch profile can't do it, and Debug numbers are meaningless.
2. Start the profile from that configuration, choosing the mode (menu wording varies by version).
3. When the app finishes, open the snapshot. Start at **Hot Spots**/**Call Tree**, sort by own time.
4. For memory: take snapshots at different points, use **Compare**, then inspect **Dominators** / **Retention paths**.

**Reading the totals in a sampling snapshot.** The root row (`100% all calls`) adds up the time of *every thread*, so it is usually much more than the time the app ran. Example: profiling L00-01 in profile mode (about 15 s) showed a root of `43,240 ms`. Three things to know:
- Rider groups digits with a separator, so `43,240 ms` is **43 seconds**, not 43 milliseconds.
- A .NET process has helper threads besides the one running `Main` (8 threads in total on the author's Linux machine, one of them doing all the work), and sampling also counts [threads that are sleeping or locked](https://www.jetbrains.com/help/profiler/Basic_Concepts.html). That is why the root is larger than the run.
- To get a number you can compare with the harness, expand the root and open the **main thread** (or filter to it). Its total should be close to the length of the run. Divide `Feed.Build`'s time on that thread by the `Done: N iterations` line the harness prints and you get the time per iteration, about 190 ms for L00-01 (the same as measure mode). The [session options](https://www.jetbrains.com/help/profiler/Profiler_Options.html) also let you choose whether time when a thread isn't working is counted.

### Visual Studio workflow
1. Set the exercise as the startup project and pick the **Profile** launch profile (command-line arguments `--profile --seconds 15`, already set up in `Properties/launchSettings.json`). Switch the solution configuration to **Release** first: the launch profile can't do it, and Debug numbers are meaningless.
2. Debug → **Performance Profiler…**, tick **CPU Usage** and/or **.NET Object Allocation Tracking**, then **Start** (not F5, which attaches a debugger and gives you meaningless numbers).
3. When it finishes, the CPU Usage view has its own call tree: sort by **Self** (own time), same idea as Rider's Hot Spots.
4. For memory: **Debug → Windows → Show Diagnostic Tools**, take **Memory Usage** snapshots before/after, then diff them to see what grew.

### CLI
One page per tool, with what to type and how to read the output: [docs/tools](tools/README.md). The short version:
```bash
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-gcdump
dotnet tool install --global dotnet-dump

dotnet-counters ps                                   # find the pid
dotnet-counters monitor -p <pid> System.Runtime      # cpu, alloc rate, gc counts/heap size, exception count, threadpool queue/threads
dotnet-trace collect -p <pid> --format Speedscope    # then open in https://www.speedscope.app
dotnet-gcdump collect -p <pid>                       # heap graph; open in VS/PerfView
dotnet-dump collect -p <pid>; dotnet-dump analyze <file>   # then: dumpheap -stat, gcroot <addr>, threads, clrstack
```

> **Counter names changed in .NET 9.** On a .NET 9+ runtime `dotnet-counters` reports `System.Runtime` metrics as `dotnet.*` names (e.g. `dotnet.thread_pool.queue.length`, `dotnet.gc.collections`, `dotnet.monitor.lock_contentions`); the names below are the .NET ≤ 8 ones. Mapping table: [levels/L08-production/README.md](../levels/L08-production/README.md).

Counters worth knowing by heart: `alloc-rate`, `gc-heap-size`, `gen-0/1/2-gc-count`, `time-in-gc`,
`exception-count`, `threadpool-queue-length`, `threadpool-thread-count`, `monitor-lock-contention-count`.

## Testing a hypothesis

The exercise harness answers *"is it fast enough, and still correct?"*. From Level 6 on, that's not enough - you also need to answer *"why is A faster than B?"*, ideally without just trusting your gut. Three tools, cheapest first.

### 1. `perf stat`: hardware counters on Linux
BenchmarkDotNet's `[HardwareCounters]` is Windows-only (per its docs), so on Linux use `perf`.
```bash
sudo dnf install perf                       # Fedora; other distros: linux-tools / perf
# perf may need:  sudo sysctl kernel.perf_event_paranoid=1   (or run with sudo)
dotnet build -c Release levels/L06-hardware-runtime/exercises/L06-01-matrix-walk
perf stat -e cycles,instructions,cache-references,cache-misses,branches,branch-misses \
  dotnet levels/L06-hardware-runtime/exercises/L06-01-matrix-walk/bin/Release/net10.0/L06-01-matrix-walk.dll --profile --seconds 5
```
Read: **IPC** (instructions / cycles: low means stalls), **cache-miss rate**, **branch-miss rate**. Compare the exercise and the solution with the same `--seconds`.
Not every counter is available in VMs/containers; if you see `<not supported>`, that hardware event isn't exposed there.

### 2. JIT disassembly: what code did you actually get?
```bash
DOTNET_JitDisasm="Run" DOTNET_JitStdOutFile=/tmp/run.asm \
  dotnet levels/L06-hardware-runtime/exercises/L06-05-transform-batch/bin/Release/net10.0/L06-05-transform-batch.dll
```
`DOTNET_JitDisasm` accepts method names/patterns (`Workload:Run`, `*Score*`). It prints the code for **each tier** a method is compiled at; look at the last (Tier-1) one.
It only shows methods *the JIT compiles*. If your code calls into the framework (`span.Count`, `IndexOf`, `Sum`), the interesting loop is in the framework method, so ask for that name (`SpanHelpers:*`, `*CountValueType*`). Framework methods start out as precompiled ReadyToRun code and only show up once they've been called often enough to be recompiled. If nothing appears, run in `--profile` mode or set `DOTNET_ReadyToRun=0`.
Things to look for: block copies before calls (defensive copies), `call [reg+…]` (indirect call, so no inlining), `vpcmpeqd`/`vpaddd` (SIMD), `cmov` (branchless) vs `jl`/`jge` (branch), bounds-check `cmp`+`jae` sequences.

### 3. BenchmarkDotNet: trustworthy micro-measurements
Use it to compare *two implementations of a small piece of code* with statistics, warm-up and process isolation handled for you.
Create a scratch project (do **not** add it to the exercise solution):
```bash
mkdir /tmp/bench && cd /tmp/bench
dotnet new console -n Bench && cd Bench
dotnet add package BenchmarkDotNet
```
`Program.cs`:
```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Walk>();

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]          // shows the generated assembly next to the numbers
public class Walk
{
    const int N = 4096;
    readonly int[] _grid = new int[N * N];

    [Benchmark(Baseline = true)]
    public long ColumnMajor() { long s = 0; for (int c = 0; c < N; c++) for (int r = 0; r < N; r++) s += _grid[r * N + c]; return s; }

    [Benchmark]
    public long RowMajor() { long s = 0; for (int r = 0; r < N; r++) for (int c = 0; c < N; c++) s += _grid[r * N + c]; return s; }
}
```
Run it: `dotnet run -c Release -- --filter '*'`. **Always Release, never under a debugger.**

### Rules that keep a benchmark honest
- **Return the result** (or consume it) so the JIT can't delete the work.
- **Same input for both versions**; build data in `[GlobalSetup]`, not in the benchmark.
- Watch the **error and StdDev** columns: if they're larger than the difference, you have no result.
- One benchmark per question; change one thing.
- Run on a quiet machine (close browsers; pin the CPU governor if you can).
- Numbers from one machine don't transfer: rerun on the target hardware.

Further reading: Akinshin, *Pro .NET Benchmarking* [[4]](./READING-LIST.md#ref4); the BenchmarkDotNet diagnosers docs [[54]](./READING-LIST.md#ref54); Bakhvalov, *Performance Analysis and Tuning on Modern CPUs* [[53]](./READING-LIST.md#ref53).

## Load, capacity and measurement rigor

The ASP.NET levels (9-14) measure a service under concurrent load, and a few ideas from queueing theory explain most of what you'll see there. They also explain why a load test can mislead you.

### Little's law and utilisation
- **Little's law:** `in-flight requests = throughput x average latency`. A service handling 200 req/s at 50 ms average latency has about 10 requests in flight. If latency doubles at the same throughput, twice as many are in flight, and that is where the thread-pool queue, the connection pool and memory grow.
- **Utilisation** is the fraction of a resource's capacity that is busy. Queueing delay grows slowly at first, then sharply: on a simple single-server queue the wait scales roughly with `u / (1 - u)`, so going from 50% to 90% busy multiplies the wait by about 9, and 95% by about 19. That is why latency looks fine until the service is *nearly* full, then collapses. Don't plan to run a shared resource (CPU, DB pool, thread pool) near 100%.
- **Saturation** is the point where a resource has no spare capacity: work arrives at least as fast as it can be served, so a queue forms and keeps growing. A resource can be saturated well below 100% CPU. A pool of 10 DB connections is saturated when all 10 are checked out and callers are waiting, and a thread pool is when its work queue is non-empty and not draining, even if the cores are idle. What it looks like from outside:
  - Throughput stops rising as you add load, and goes flat (or falls) while latency keeps climbing.
  - A queue grows: pool wait time, `threadpool-queue-length`, pending requests, a rising in-flight count (Little's law again).
  - Utilisation of one resource sits near its limit while the others are comfortable.

  Utilisation tells you how busy a resource is, and saturation tells you whether work is already waiting for it. Check both for each resource (CPU, memory, thread pool, connection pool, disk/network, downstream service), and look for *queues* rather than just busy percentages. Saturation is also what separates the two ways to be slow: an unsaturated service is slow because each request does too much work (profile the code), a saturated one is slow because requests are waiting (find the queue and the resource it's waiting for).
- **The bottleneck sets the ceiling.** Throughput is capped by the most saturated resource. Adding capacity anywhere else changes nothing, so find the saturated one first (CPU, pool size, a downstream dependency, a lock).
- **Once arrivals exceed capacity the queue grows without bound**, and latency rises for as long as the overload lasts. Retries and timeouts make it worse by adding load exactly when there is none to spare (the L12 retry storm).

### What the load driver can lie about
- **Closed loop vs open loop.** A closed-loop driver (N users, each sends the next request when the last one returns; `WebRig.Drive`) slows down when the service slows down, so it never overloads it and hides queueing. An open-loop driver (requests arrive at a fixed rate whatever the service does; `WebRig.DriveOpen`) is what real traffic looks like, and it is the one that exposes saturation. Use closed loop to find best-case latency, open loop to find where it breaks.
- **Coordinated omission.** If a stall makes the driver skip the requests it should have sent, those missing slow requests never enter the statistics and the tail looks better than it was. Measure latency from when the request was *due*, not when it was actually sent. `DriveOpen` does this.
- **Averages hide the tail.** Report p50, p99 and max, and the number of requests behind each. A p99 from 100 samples is one request. Percentiles can't be averaged across runs or instances; merge the raw samples or histograms instead.
- **Warm up, then measure.** Discard the start (JIT, caches, pool growth) and say how long the run was. Short runs under-sample rare events such as gen2 GCs.

### Measurement checklist
1. State the question and the number that answers it (p99 at 500 req/s, not "faster").
2. Repeat the run, and compare the spread to the difference you are trying to detect. If the noise is bigger than the effect, you have no result.
3. Change one thing, keep the workload identical, and record the machine state (load, power mode, Release vs Debug).
4. Check that the result is *correct* as well as fast (the harness checksum), and that errors and timeouts are counted rather than dropped.
5. Confirm the finding with a second method before you act on it (see [Traps](#traps)).

Further reading: Gil Tene's talk "How NOT to Measure Latency" (coordinated omission); Google, *Site Reliability Engineering*, the chapter on monitoring distributed systems (latency and the four golden signals).