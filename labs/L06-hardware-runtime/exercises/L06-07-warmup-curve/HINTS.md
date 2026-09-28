# Hints (open one at a time)

<details><summary>Hint 1: the mechanism</summary>

.NET starts every method in a fast-to-compile, slow-to-run **tier 0** (or uses precompiled *ReadyToRun* code), counts calls and loop iterations, and later recompiles hot methods with the optimising JIT (**tier 1**), using profile data gathered along the way (dynamic PGO). Run 1 pays for the slow start; later runs benefit from the promoted code.
</details>

<details><summary>Hint 2: the trade</summary>

Turning tiering off compiles everything with the optimiser immediately: no slow tier-0 code, but *every* method (including ones you'll never run twice) pays full JIT cost at startup, and you lose PGO's steady-state gains.
</details>

<details><summary>Hint 3: what to look at</summary>

Use `DOTNET_JitStdOutFile` with `DOTNET_JitDisasmSummary=1` to list which methods were compiled at which tier and in what order. `dotnet-counters` shows `methods-jitted-count` and `time-in-jit` for the process.
</details>
