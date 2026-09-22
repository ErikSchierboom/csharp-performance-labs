# L7-04 · Observer effect

## Symptom
A lookup loop over 2 million keys takes about **0.5 s**. **Tracing** mode blames the tiny `IsValid` helper (millions of calls) and suggests inlining it by hand. **Sampling** mode disagrees: it shows most of the time in dictionary lookup, `Equals` and the key's hash. Only one of the two profiles is pointing at the real problem.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 52 ref-ms |
| Median allocated | 1 MB |

## Boss fight rules
- **Symptom only.** There are several defects and fixing one usually exposes the next; re-measure after every change.
- Hints are deliberately generic. Use `templates/POSTMORTEM.md` and write the post-mortem *before* you read the solution.
- Budgets are on several metrics at once; hitting one is not enough.

Profile this one **twice, in two modes** (sampling and tracing) and write down where each one points. They will disagree.

## Extra credit
Use `[MethodImpl(MethodImplOptions.NoInlining)]` on `IsValid` in a scratch copy and re-run *sampling*. Does the picture move toward the tracing one? What does that teach?
