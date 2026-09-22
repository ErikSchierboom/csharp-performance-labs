# Runtime: how .NET runs your code

Every exercise in this lab is timed after a warm-up, and the harness has a `--cold` mode with no warm-up at all. This page explains why, because it changes how you read every number you'll see. Read it once in Level 0; you'll come back to it in Level 6 ([L6-07 warm-up curve](../levels/L06-hardware-runtime/exercises/L6-07-warmup-curve/README.md) is the hands-on follow-up).

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
One method with a hot loop (`Score`, below), timed in eight successive batches of 20 calls, with the ladder switched on and off. Milliseconds per batch; two runs each, which agreed to within about 0.3 ms:

| Configuration | Setting | Batch 1 | Batches 2–8 |
|---|---|---|---|
| Everything on (default) | none | 1.3 | 0.9 |
| No PGO instrumentation | `DOTNET_TieredPGO=0` | 1.1 | 0.8–0.9 |
| Loops skip Tier 0 | `DOTNET_TC_QuickJitForLoops=0` | 0.8 | 0.7 |
| Tiering off (`FullOpts`) | `DOTNET_TieredCompilation=0` | 0.8 | 0.7–0.8 |
| Stuck in Tier 0 (a diagnostic trick, see below) | `DOTNET_TC_QuickJitForLoops=1 DOTNET_OSR_HitLimit=1000000 DOTNET_TC_BackgroundWorkerTimeoutMs=999999` | 3.6–3.7 | 3.1–3.4 |

Read it like this:
- **Unoptimised code is several times slower.** Code that never leaves Tier 0 runs about **4×** slower than the fully optimised version here (3.1 vs 0.7 ms). That is what a program runs on until the runtime decides a method is hot.
- **The default is not the fastest at first.** It starts at 1.3 ms and settles at 0.9, while `FullOpts` is at 0.7 from the first batch. Start-up cost is what you pay for the ladder; on a short run you can see the price and not yet the benefit.
- **The default keeps improving, given time.** Run the same program for longer and the default eventually catches up. Timed with 600 batches, the default started at 1.3 ms, ran at about 0.9 ms, and dropped to **0.7 ms about 200 ms in**, when the method reached Tier 1.
- The "stuck in Tier 0" row uses settings that block promotion; it is a way to *see* Tier 0, not a supported configuration, and the setting names and effects can change between .NET releases.

Reproduce it. Create a scratch console project (`dotnet new console`) and use this `Program.cs`:
```csharp
using System.Diagnostics;

long acc = 0;
int batches = args.Length > 0 ? int.Parse(args[0]) : 8;
var rows = new List<string>();
for (int b = 0; b < batches; b++)
{
    long t = Stopwatch.GetTimestamp();
    for (int k = 0; k < 20; k++) acc += Score(50_000);
    rows.Add(Stopwatch.GetElapsedTime(t).TotalMilliseconds.ToString("F1"));
}
if (acc == 42) Console.WriteLine(acc);           // use the result so the work can't be discarded
Console.WriteLine(string.Join(" ", rows));

static long Score(int n) { long s = 0; for (int i = 0; i < n; i++) s += (i * 7) % 101; return s; }
```
```bash
dotnet build -c Release
dotnet bin/Release/net10.0/<name>.dll                                   # everything on
DOTNET_TieredPGO=0 dotnet bin/Release/net10.0/<name>.dll                 # no instrumentation
DOTNET_TieredCompilation=0 dotnet bin/Release/net10.0/<name>.dll        # tiering off
dotnet bin/Release/net10.0/<name>.dll 600                                # long run: watch it settle
```
(On Linux, `taskset -c 0,2 dotnet …` keeps the run on two cores, which makes the numbers steadier. Do not use a single core for this: see "How long does promotion take?" below.)

## Watching it happen: `DOTNET_JitDisasmSummary`
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
Read the last line: that is where the method ends up. It went from an instrumented, unoptimised body (169 bytes of code) through an on-stack-replacement version (80) to the final Tier 1 body that was compiled using the recorded profile (56). With `DOTNET_TieredCompilation=0` the same method is one line, `[FullOpts]`. (The repeated lines are the runtime's own route through the stages; treat the exact sequence as an implementation detail.)

Run the built `.dll` directly, as above. If you use `dotnet run`, the SDK's own start-up code is compiled into the same file and buries what you're looking for.

The same switch shows how much work ReadyToRun saves. For a trivial program like this one, the default run compiled **73** methods with the JIT. With `DOTNET_ReadyToRun=0`, which makes the runtime ignore precompiled code, it compiled **671**. Nearly everything the program touched in the framework was already compiled. How much that is worth in time is the subject of [L6-07](../levels/L06-hardware-runtime/exercises/L6-07-warmup-curve/README.md).

## How long does promotion take?
Not immediately, and by design. The runtime waits until start-up looks finished before it counts calls: a timer of 100 ms starts, and **any new Tier 0 compilation resets it**; only when it expires quietly do calls get counted, and a method needs about 30 calls to become eligible [[106]](READING-LIST.md#ref106). With the instrumented stage in between, that means **roughly 200 ms** before a hot method is at Tier 1. We saw exactly that. The loop method above dropped at 201 ms; in a second test program (a small method that is not inlined, called about two million times per batch) the drop appeared at 196–205 ms whether the run was unpinned, on two cores or on four.

**On a single CPU the delay is ten times longer.** Pinned to one core (`taskset -c 0`), that second program did not reach Tier 1 until about **2,000 ms**, in all three runs. Setting `DOTNET_TC_DelaySingleProcMultiplier=1` brought it back to about 205 ms, which is how we know the multiplier is the cause. A container limited to one CPU is likely to behave the same way, because the runtime sees one processor; we only tested with `taskset`, so check yours. Anything you time on a one-core box needs a far longer warm-up.

## What this means for the rest of the lab
- **Warm-up is not cheating.** A one-shot run measures start-up. The harness keeps running the workload until at least two runs *and* one second have passed, up to 60 runs, and only then times it. `--cold` skips that so you can watch the ladder: `DOTNET_TieredPGO=0 dotnet run -c Release --project <exercise> -- --cold --runs 10`.
- **A fast workload can still be measured before Tier 1.** The 60-run cap ends the warm-up early when each run is quick. We checked one: the L0-01 solution runs in about 0.7 ms, so 60 warm-up runs take a few tens of milliseconds, well under the ~200 ms that Tier 1 needs. Its own listing shows `Feed.Build` finishing the whole run as a `Tier1-OSR` variant and small methods such as `FeedItem.get_Id` still at Tier 0. The budgets have plenty of headroom, so the verdicts are unaffected, but this is why you leave headroom in a budget you write for a very fast workload.
- **Profile warmed-up code.** A profile of a process that ran for 200 ms is mostly a profile of Tier 0 code and the JIT itself. `--profile` mode loops for seconds for that reason.
- **Dynamic PGO can change what the fast code looks like.** Because Tier 1 can use the recorded profile, hot paths can be compiled with bets a cold compiler wouldn't make, such as guarded calls for a virtual method that almost always sees one type. [[55]](READING-LIST.md#ref55) has the details, and Level 6 comes back to dispatch.
- **The settings are for learning, not for shipping.** Switching tiers off or blocking promotion is a way to *see* the ladder. Don't run production with them off; there are documented, supported settings for production [[105]](READING-LIST.md#ref105).

## Check yourself
1. Why doesn't the runtime compile every method with the full optimiser the first time it is called?
2. On the table above, what does the default cost you on a short run, and what does it buy you on a long one?
3. What is on-stack replacement for, and why does a method with a long loop need it?
4. Why would a benchmark that runs for 50 ms on a one-core machine tell you almost nothing about the fast code?

## Further reading
[[105]](READING-LIST.md#ref105) Compilation config settings · [[106]](READING-LIST.md#ref106) Tiered compilation design · [[55]](READING-LIST.md#ref55) Dynamic PGO design · [[56]](READING-LIST.md#ref56) OSR details · [[102]](READING-LIST.md#ref102) ReadyToRun · [[2]](READING-LIST.md#ref2) Toub's yearly *Performance Improvements in .NET*, whose yearly editions discuss these features
