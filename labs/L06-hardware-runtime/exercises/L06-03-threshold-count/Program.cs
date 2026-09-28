using ThresholdCount;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-03-threshold-count",
    Workload: Workload.Run,
    ExpectedChecksum: 5743735661140980,
    MaxMetrics: new() { [Metrics.Time] = 60, [Metrics.Alloc] = 1 }), args);
