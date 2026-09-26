# Runtime: how .NET runs your code

>This page is about how the runtime turns your methods into machine code. The rest of .NET (the base class library, the thread pool, ASP.NET Core) is covered elsewhere: the thread pool and async in [Level 4](../ROADMAP.md) and, under load, [Level 10](../ROADMAP.md).

Every exercise in this lab is timed after a warm-up, and the harness has a `--cold` mode with no warm-up at all. This page explains why, because it changes how you read every number you'll see. Read it once in Level 0; you'll come back to it in Level 6 ([L06-07 warm-up curve](../levels/L06-hardware-runtime/exercises/L06-07-warmup-curve/README.md) is the hands-on follow-up).


**Where the numbers come from.** The measurements on this page were taken on the author's machine (.NET 10.0.111, Linux, Release build, pinned to two CPUs) and are there to show the *shape* of the effect. Yours will differ. How each stage works is described by the runtime's own documentation [[105]](READING-LIST.md#ref105), [[106]](READING-LIST.md#ref106), [[55]](READING-LIST.md#ref55), [[56]](READING-LIST.md#ref56), [[102]](READING-LIST.md#ref102); go to them for the details.

## The idea: start cheap, spend more on what turns out to be hot
.NET code is compiled to machine code by the JIT (just-in-time compiler) the first time a method runs, and most methods in a program run once or never again. Optimising every method as aggressively as possible would make every start-up slow. So the runtime keeps several ways of running the same method and moves hot code up a ladder:

| Stage | What it is | Speed of the code it makes |
|---|---|---|
| **ReadyToRun (R2R)** | Machine code compiled ahead of time and shipped inside the assembly [[102]](READING-LIST.md#ref102). The framework itself is built this way, so most of the framework never needs the JIT at start-up | Decent, but not fully optimised |
| **Tier 0** | The JIT in a hurry: compiles fast, does almost no optimisation [[105]](READING-LIST.md#ref105) | Slowest |
| **Instrumented Tier 0** | Tier 0 plus counters that record how the method actually behaves: which branches are taken, which concrete types show up [[55]](READING-LIST.md#ref55) | Slow (the counters cost) |
| **OSR** (on-stack replacement) | For a method that is *already running* a long loop: swaps the running loop into optimised code part-way through the call, instead of waiting for the next call [[56]](READING-LIST.md#ref56) | Optimised |
| **Tier 1** | The full optimising JIT, using the recorded profile when there is one (*Dynamic PGO*, profile-guided optimisation) [[55]](READING-LIST.md#ref55) | Fastest |

Turning tiering off (`DOTNET_TieredCompilation=0`) skips the ladder: every method is compiled with full optimisation the first time it is called. That is the `FullOpts` label you'll see in the listing below.

The trade-off is the whole point. Tier 0 gets your program running quickly; Tier 1 gets it running *fast*; the profile from the instrumented stage lets Tier 1 make bets (for example "this virtual call almost always lands on the same type") that a compiler with no data couldn't.

## What it looks like: one method, five ways
One method with a hot loop (`Score`, below), timed in eight successive batches of 20 calls, with the ladder switched on and off. Microseconds per batch; each point is the median of five runs.

````mermaid

xychart
    title "Effects of tiered compilation"
    x-axis "batch" [1, 2, 3, 4, 5, 6, 7, 8]
    y-axis "Execution time (µs)" 500 --> 4000
    line "Default" [1324,960,938,931,929,923,914,921]
    line "No PGO" [1109,881,866,872,874,871,881,873]
    line "No tiered compilation" [810,689,684,680,678,680,676,683]
    line "Stuck in Tier 0" [1340,1290,1293,1310,1302,1295,1293,1285]
    line "Stuck in instrumented Tier 0" [3589,3166,3120,3118,3094,3173,3076,3094]

````


- **The default is not the fastest at first.** Starts slow and improves, but hasn't reached Tier 1 within 8 batches (about 8 ms of runtime). Promotion takes about 200 ms, so in a longer run it drops to about 680 µs, the same as "no tiered compilation".
- **No PGO skips the counters.** The ladder without the instrumented stage is slightly faster early on because it isn't paying for the counters, but its Tier 1 code gets no profile to work from. For this method that makes no difference in the end (a simple arithmetic loop gives PGO nothing to bet on); for virtual calls it can, and Level 6 comes back to that.
- **No tiered compilation is fast from the start.** Every method gets the full optimiser the first time it's called. Batch 1 is a little slower because it includes one-time costs such as compiling the method.
- **Unoptimised code is about twice as slow.** With promotion blocked (stuck in plain, *uninstrumented* Tier 0), the method never leaves Tier 0 and runs about 2 times slower than the fully optimised version. That is what a program runs on until the runtime decides a method is hot.
- **The counters cost more than the missing optimisation.** Stuck in *instrumented* Tier 0, the same method is about 4.5 times slower than optimised code. For a method with a loop this is where the default starts (the listing below shows it), which is why the default's first batches are slower than No PGO's.
- The "stuck" lines use settings that block promotion; they are a way to *see* Tier 0, not a supported configuration, and the setting names and effects can change between .NET releases.

Reproduce it. Create a scratch console project (`dotnet new console -o Scratch`) and use this `Program.cs`:
```csharp
using System.Diagnostics;

long acc = 0;
int batches = args.Length > 0 ? int.Parse(args[0]) : 8;
var rows = new List<string>();
for (int b = 0; b < batches; b++)
{
    long t = Stopwatch.GetTimestamp();
    for (int k = 0; k < 20; k++) acc += Score(50_000);
    rows.Add(Stopwatch.GetElapsedTime(t).TotalMicroseconds.ToString("F0"));
}
if (acc == 42) Console.WriteLine(acc);           // use the result so the work can't be discarded
Console.WriteLine(string.Join(" ", rows));

static long Score(int n) { long s = 0; for (int i = 0; i < n; i++) s += (i * 7) % 101; return s; }
```
```bash
dotnet build -c Release
dotnet bin/Release/net10.0/Scratch.dll                                                          # everything on
DOTNET_TieredPGO=0 dotnet bin/Release/net10.0/Scratch.dll                                       # no instrumentation
DOTNET_TieredCompilation=0 dotnet bin/Release/net10.0/Scratch.dll                               # tiering off: straight to full optimisation
DOTNET_TC_CallCounting=0 DOTNET_TC_OnStackReplacement=0 dotnet bin/Release/net10.0/Scratch.dll  # stuck in Tier 0: no call counting, no OSR
DOTNET_TC_CallCountingDelayMs=7FFFFFFF DOTNET_TC_OnStackReplacement_InitialCounter=7FFFFFFF dotnet bin/Release/net10.0/Scratch.dll # stuck in instrumented Tier 0: counting never starts
dotnet bin/Release/net10.0/Scratch.dll 600                                                      # long run: watch it settle
```
`DOTNET_*` values are read as **hexadecimal**: `7FFFFFFF` is the largest value, and writing it in decimal (`2147483647`) gives a different number that does not block promotion.

(On Linux, `taskset -c 0,2 dotnet …` keeps the run on two cores, which makes the numbers steadier. Do not use a single core for this: see "How long does promotion take?" below.)

## Watching it happen
The runtime will list every method it compiles, and at which stage:
```bash
DOTNET_JitStdOutFile=/tmp/jit.txt DOTNET_JitDisasmSummary=1 dotnet bin/Release/net10.0/<name>.dll 600
grep Score /tmp/jit.txt
```
For the 600-batch run above this printed (shortened: because `Score` is a local function, the full name in the file is `Program:<<Main>$>g__Score|0_0(int)`):
```text
Score(int) [Instrumented Tier0, IL size=27, code size=169]
Score(int) [Tier1-OSR @0x15 with Synthesized PGO, IL size=27, code size=80]
Score(int) [Instrumented Tier0, IL size=27, code size=169]
Score(int) [Tier1-OSR @0x15 with Synthesized PGO, IL size=27, code size=80]
Score(int) [Tier1 with Dynamic PGO, IL size=27, code size=56]
```
Read the last line: that is where the method ends up. It went from an instrumented, unoptimised body (169 bytes of code) through an on-stack-replacement version (80) to the final Tier 1 body that was compiled using the recorded profile (56).

Run the built `.dll` directly, as above. If you use `dotnet run`, the SDK's own start-up code is compiled into the same file and buries what you're looking for.

The same switch shows how much work ReadyToRun saves. For a trivial program like this one, the default run compiled **73** methods with the JIT. With `DOTNET_ReadyToRun=0`, which makes the runtime ignore precompiled code, it compiled **671**. Nearly everything the program touched in the framework was already compiled. How much that is worth in time is the subject of [L06-07](../levels/L06-hardware-runtime/exercises/L06-07-warmup-curve/README.md).

## How long does promotion take?
Not immediately, and by design. The runtime waits until start-up looks finished before it counts calls: a timer of 100 ms starts, and **any new Tier 0 compilation resets it**; only when it expires quietly do calls get counted, and a method needs about 30 calls to become eligible [[106]](READING-LIST.md#ref106). With the instrumented stage in between, that means **roughly 200 ms** before a hot method is at Tier 1. We saw exactly that. The loop method above dropped at 201 ms; in a second test program (a small method that is not inlined, called about two million times per batch) the drop appeared at 196–205 ms whether the run was unpinned, on two cores or on four.

**On a single CPU the delay is ten times longer.** Pinned to one core (`taskset -c 0`), that second program did not reach Tier 1 until about **2,000 ms**, in all three runs. Setting `DOTNET_TC_DelaySingleProcMultiplier=1` brought it back to about 205 ms, which is how we know the multiplier is the cause. A container limited to one CPU is likely to behave the same way, because the runtime sees one processor; we only tested with `taskset`, so check yours. Anything you time on a one-core box needs a far longer warm-up.

## What this means for the rest of the lab
- **Warm-up is not cheating.** A one-shot run measures start-up. The harness keeps running the workload until at least two runs *and* one second have passed, up to 60 runs, and only then times it. `--cold` skips that so you can watch the ladder: `DOTNET_TieredPGO=0 dotnet run -c Release --project <exercise> -- --cold --runs 10`.
- **A fast workload can still be measured before Tier 1.** The 60-run cap ends the warm-up early when each run is quick. We checked one: the L00-01 solution runs in about 0.7 ms, so 60 warm-up runs take a few tens of milliseconds, well under the ~200 ms that Tier 1 needs. Its own listing shows `Feed.Build` finishing the whole run as a `Tier1-OSR` variant and small methods such as `FeedItem.get_Id` still at Tier 0. The budgets have plenty of headroom, so the verdicts are unaffected, but this is why you leave headroom in a budget you write for a very fast workload.
- **Profile warmed-up code.** A profile of a process that ran for 200 ms is mostly a profile of Tier 0 code and the JIT itself. `--profile` mode loops for seconds for that reason.
- **Dynamic PGO can change what the fast code looks like.** Because Tier 1 can use the recorded profile, hot paths can be compiled with bets a cold compiler wouldn't make, such as guarded calls for a virtual method that almost always sees one type. [[55]](READING-LIST.md#ref55) has the details, and Level 6 comes back to dispatch.
- **The settings are for learning, not for shipping.** Switching tiers off or blocking promotion is a way to *see* the ladder. Don't run production with them off; there are documented, supported settings for production [[105]](READING-LIST.md#ref105).

## Check yourself
1. Why doesn't the runtime compile every method with the full optimiser the first time it is called?
2. On the chart above, what does the default cost you on a short run, and what does it buy you on a long one?
3. What is on-stack replacement for, and why does a method with a long loop need it?
4. Why would a benchmark that runs for 50 ms on a one-core machine tell you almost nothing about the fast code?

## Further reading
[[105]](READING-LIST.md#ref105) Compilation config settings · [[106]](READING-LIST.md#ref106) Tiered compilation design · [[55]](READING-LIST.md#ref55) Dynamic PGO design · [[56]](READING-LIST.md#ref56) OSR details · [[102]](READING-LIST.md#ref102) ReadyToRun · [[2]](READING-LIST.md#ref2) Toub's yearly *Performance Improvements in .NET*, whose yearly editions discuss these features
