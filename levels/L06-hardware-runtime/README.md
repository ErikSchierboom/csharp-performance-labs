# Level 6: hardware & runtime effects

Same algorithm, same allocation count, and one version is still five times slower, because of how the CPU and the JIT actually execute it, which is not always what the code implies on paper.

**Skills:** Cache locality, branch prediction, false sharing, struct copies, dispatch, tiered JIT, SIMD.

**Mastery checkpoint:** Predict which of two implementations is faster before measuring, and be right for the right reason.

Run one: `dotnet run -c Release --project levels/L06-hardware-runtime/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 6 reading list](../../docs/READING-LIST.md#level-6-hardware-runtime-effects)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L6-01-matrix-walk](exercises/L6-01-matrix-walk/README.md) | Matrix walk (cache locality) | [solution](solutions/L6-01-matrix-walk/SOLUTION.md) |
| [L6-02-particle-sweep](exercises/L6-02-particle-sweep/README.md) | Particle sweep (pointer chasing) | [solution](solutions/L6-02-particle-sweep/SOLUTION.md) |
| [L6-03-threshold-count](exercises/L6-03-threshold-count/README.md) | Threshold count (branch misprediction) | [solution](solutions/L6-03-threshold-count/SOLUTION.md) |
| [L6-04-worker-counters](exercises/L6-04-worker-counters/README.md) | Worker counters (false sharing) | [solution](solutions/L6-04-worker-counters/SOLUTION.md) |
| [L6-05-transform-batch](exercises/L6-05-transform-batch/README.md) | Transform batch (hidden struct copies) | [solution](solutions/L6-05-transform-batch/SOLUTION.md) |
| [L6-06-shape-areas](exercises/L6-06-shape-areas/README.md) | Shape areas (dispatch) | [solution](solutions/L6-06-shape-areas/SOLUTION.md) |
| [L6-07-warmup-curve](exercises/L6-07-warmup-curve/README.md) | Warm-up curve  (an exploration, no pass/fail gate) | [solution](solutions/L6-07-warmup-curve/SOLUTION.md) |
| [L6-08-count-matches](exercises/L6-08-count-matches/README.md) | Count matches (vectorisation) | [solution](solutions/L6-08-count-matches/SOLUTION.md) |
| [L6-10-bit-counts](exercises/L6-10-bit-counts/README.md) | Bit counts (hardware intrinsics) | [solution](solutions/L6-10-bit-counts/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L6-boss-sensor-grid](exercises/L6-boss-sensor-grid/README.md) | Sensor grid (final boss of Level 6) | [solution](solutions/L6-boss-sensor-grid/SOLUTION.md) |
