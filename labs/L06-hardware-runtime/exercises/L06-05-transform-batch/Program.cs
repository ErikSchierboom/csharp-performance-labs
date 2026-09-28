using TransformBatch;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-05-transform-batch",
    Workload: Workload.Run,
    ExpectedChecksum: 210000000,
    MaxMetrics: new() { [Metrics.Time] = 40, [Metrics.Alloc] = 0 }), args);
