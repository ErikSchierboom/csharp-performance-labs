# Lab 6: hardware & runtime effects

Same algorithm, same allocation count, and one version is still five times slower, because of how the CPU and the JIT actually execute it, which is not always what the code implies on paper.

**Skills:** Cache locality, branch prediction, false sharing, struct copies, dispatch, tiered JIT, SIMD.

**Mastery checkpoint:** Predict which of two implementations is faster before measuring, and be right for the right reason.

Run one: `dotnet run -c Release --project labs/L06-hardware-runtime/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 6 reading list](../../docs/READING-LIST.md#lab-6-hardware-runtime-effects)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L06-01-matrix-walk](exercises/L06-01-matrix-walk/README.md) | Matrix walk | [solution](solutions/L06-01-matrix-walk/SOLUTION.md) |
| [L06-02-particle-sweep](exercises/L06-02-particle-sweep/README.md) | Particle sweep | [solution](solutions/L06-02-particle-sweep/SOLUTION.md) |
| [L06-03-threshold-count](exercises/L06-03-threshold-count/README.md) | Threshold count | [solution](solutions/L06-03-threshold-count/SOLUTION.md) |
| [L06-04-worker-counters](exercises/L06-04-worker-counters/README.md) | Worker counters | [solution](solutions/L06-04-worker-counters/SOLUTION.md) |
| [L06-05-transform-batch](exercises/L06-05-transform-batch/README.md) | Transform batch | [solution](solutions/L06-05-transform-batch/SOLUTION.md) |
| [L06-06-shape-areas](exercises/L06-06-shape-areas/README.md) | Shape areas | [solution](solutions/L06-06-shape-areas/SOLUTION.md) |
| [L06-07-warmup-curve](exercises/L06-07-warmup-curve/README.md) | Warm-up curve  (an exploration, no pass/fail gate) | [solution](solutions/L06-07-warmup-curve/SOLUTION.md) |
| [L06-08-count-matches](exercises/L06-08-count-matches/README.md) | Count matches | [solution](solutions/L06-08-count-matches/SOLUTION.md) |
| [L06-09-bit-counts](exercises/L06-09-bit-counts/README.md) | Bit counts | [solution](solutions/L06-09-bit-counts/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L06-boss-sensor-grid](exercises/L06-boss-sensor-grid/README.md) | Sensor grid | [solution](solutions/L06-boss-sensor-grid/SOLUTION.md) |
