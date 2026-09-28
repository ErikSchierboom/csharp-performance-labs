# Lab 0: a worked example, every template filled in

Use this once, before Lab 1, to see the *shape* of a finished attempt, what "done" actually looks like, before you're the one trying to produce it. It uses the exercise
[`labs/L00-start-here/exercises/L00-01-activity-feed`](../../labs/L00-start-here/exercises/L00-01-activity-feed/README.md). Spoilers below, so **try the exercise first for 15 minutes**, then compare.

| Template | Where it lives | The filled-in example |
|---|---|---|
| Symptom + budgets | exercise `README.md` | [../../labs/L00-start-here/exercises/L00-01-activity-feed/README.md](../../labs/L00-start-here/exercises/L00-01-activity-feed/README.md) |
| Progressive hints | exercise `HINTS.md` | [../../labs/L00-start-here/exercises/L00-01-activity-feed/HINTS.md](../../labs/L00-start-here/exercises/L00-01-activity-feed/HINTS.md) |
| Lab-log entry | your `templates/LAB-LOG.md` | [LAB-LOG-example.md](LAB-LOG-example.md) |
| Solution write-up | `solutions/…/SOLUTION.md` | [../../labs/L00-start-here/solutions/L00-01-activity-feed/SOLUTION.md](../../labs/L00-start-here/solutions/L00-01-activity-feed/SOLUTION.md) |
| Post-mortem (Lab 7+ template, shown early) | your `templates/POSTMORTEM.md` | [POSTMORTEM-example.md](POSTMORTEM-example.md) |

## The loop, at a glance
1. `dotnet run -c Release --project labs/L00-start-here/exercises/L00-01-activity-feed` = `FAIL` (≈ 190 ms vs a 50-ref-ms budget).
2. Profile with sampling (`--profile --seconds 15`). Sort by **own time**. Find the top frame, then the first frame that is *your* code.
3. **Write the hypothesis in the log before touching code.**
4. Change **one** thing. Re-run. `PASS`.
5. Explain *why the number moved*. Read the solution. Break your own fix.

## What a good entry is (and isn't)
- Specific: it names a frame, a count, a size.
- Honest: it records the wrong first guess. That is where calibration comes from.
- Short: 10 lines, not a report.
