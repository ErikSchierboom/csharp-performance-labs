# L08-05 - A performance gate in CI

The harness's budgets are already a gate. In this lab you make them **automatic**, and decide what's worth gating and how to keep it trustworthy.

## Part 1: run the gate
```powershell
./scripts/perf-gate.ps1 L01         # builds, then checks: every solution PASSES, every exercise FAILS
```
A green run means the budgets *still separate* fixed code from slow code. A `SURPRISE` means either a regression in a solution, or a budget that no longer bites.

## Part 2: break something on purpose
Pick a solution and introduce a regression (examples below), run `./scripts/perf-gate.ps1 <prefix>`, and confirm it goes red. Then say *which budget* caught it and whether that's the right one.
- `levels/L02-allocations/solutions/L02-05-price-lookup`: change `ValueTask<decimal>` back to `async Task<decimal>` (allocation gate).
- `levels/L04-async-concurrency/solutions/L04-04-config-cache`: change `Lazy<int>` back to a plain value (CPU and `factoryCalls` gates).
- `levels/L01-hot-spots/solutions/L01-03-log-classifier`: construct the `Regex` per call again.
- A **silent** one: in `levels/L02-allocations/solutions/L02-03-page-renderer`, remove the `Clear()`. Which gate catches it? (Correctness, exit 2.)

## Part 3: design the gate
Answer in [QUESTIONS.md](QUESTIONS.md): what to gate, how to set budgets from an SLO, and how to handle noise.

Compare with [ANSWERS.md](ANSWERS.md). Also see [.github/workflows/perf-gate.yml](../../../.github/workflows/perf-gate.yml) for a workflow example.
