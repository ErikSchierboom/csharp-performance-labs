# L06-07 - Warm-up curve

*First Impressions*

## Symptom
The same workload takes **~17 ms the first time and ~6.5 ms every time after**. In production that first slow call is what the first user after a deploy (or the first request to a freshly scaled-out instance) experiences.
The steady-state numbers are great; the *first* ones aren't. Is this JIT compilation, tiering, or something else, and what can you change about it?

## What this exercise is
There is **no budget** to hit. The harness never warms up in this mode. Instead you run the workload cold and study the curve:

```bash
dotnet run -c Release --project labs/L06-hardware-runtime/exercises/L06-07-warmup-curve -- --cold --runs 10
```

## Predict first
Write your predictions in `templates/LAB-LOG.md` **before** running. Then run the workload under each setting and record the run-1 and run-8 times:

| Setting (environment variable) | What it changes | Your prediction | Measured run 1 / run 8 |
|---|---|---|---|
| *(default)* | tiered compilation + dynamic PGO + ReadyToRun framework | | |
| `DOTNET_TieredPGO=0` | no instrumentation tier; less PGO-driven optimisation | | |
| `DOTNET_TieredCompilation=0` | every method fully optimised at first JIT | | |
| `DOTNET_TC_QuickJitForLoops=0` | methods with loops skip tier-0 | | |
| `DOTNET_ReadyToRun=0` | ignore precompiled framework code; JIT everything | | |

```bash
DOTNET_TieredPGO=0 dotnet run -c Release --project labs/L06-hardware-runtime/exercises/L06-07-warmup-curve -- --cold --runs 10
```

## Questions to answer in your log
1. Why is run 1 slower than run 2, and why does run 2 look the same as run 8?
2. Which setting improved **run 1**, and what did it cost in **steady state**? Why is there no free lunch?
3. When would you accept a slow first request for faster steady state? When would you not? (Think: batch job vs. autoscaling web service vs. CLI tool.)
4. `DOTNET_ReadyToRun=0` makes even the later runs slower. Why? What does that say about how much of your app's time is framework code that arrives *precompiled*?
