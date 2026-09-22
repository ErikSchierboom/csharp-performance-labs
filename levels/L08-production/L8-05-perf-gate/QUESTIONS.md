# Questions
1. **What do you gate?** For each metric: allocation per run, CPU/wall time, GC counts, retained memory, p99 latency. Say whether it is *exact* or *noisy*, and whether it should block a merge, warn, or just be tracked over time.
2. **From SLO to budget.** Your service's SLO is "p99 < 300 ms at 200 requests/second on 4 cores". A baseline run shows p99 = 180 ms and 1.2 MB allocated per request. Write the budgets you'd commit for: p99, allocated bytes per request, and error rate. How much headroom, and why?
3. **Noise.** Three ways a time-based gate goes flaky (shared runners, thermal state, tiered JIT, other tenants). For each, name the mitigation this harness already uses (or would need).
4. **Re-run policy.** A gate fails once and passes on retry. What do you do (retry silently? retry twice? require N consecutive failures?), and what does that policy cost you?
5. **Gaming.** How could someone make the gate green without improving anything (e.g. raising the budget, weakening the workload)? What review rule prevents it?
6. **Baseline drift.** Budgets get looser over time and nobody notices. Propose a mechanism (e.g. a ratchet: budgets can only tighten without an explicit, reviewed exception).
