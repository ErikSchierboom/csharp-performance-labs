using MatrixWalk;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-01-matrix-walk",
    Workload: Workload.Run,
    ExpectedChecksum: 8378823002,
    MaxMetrics: new() { [Metrics.Time] = 10, [Metrics.Alloc] = 0 }), args);
