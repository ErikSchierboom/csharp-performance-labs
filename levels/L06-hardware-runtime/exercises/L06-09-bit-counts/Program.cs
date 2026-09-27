using BitCounts;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-09-bit-counts",
    Workload: Workload.Run,
    ExpectedChecksum: 128004368,
    MaxMetrics: new() { [Metrics.Time] = 5, [Metrics.Alloc] = 0 }), args);
