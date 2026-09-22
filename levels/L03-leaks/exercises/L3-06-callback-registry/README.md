# L3-06 · Callback registry (closures capture more than you think)

## Symptom
3,000 small callbacks are registered in a long-lived registry. Each returns one number, but after the run about **60 MB stays reachable after a full GC**: 20 KB per callback. The registry is bounded by design here (test scaffolding clears it). The question is why each entry is so *large*.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 8 ref-ms |
| Median allocated | 144 MB |
| Kept after a full GC | ≤ 2 MB |

## Extra credit
What if the lambda captured `this` instead? What would be retained?
