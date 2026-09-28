using BatchReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-04-batch-report",
    Workload: Workload.Run,
    ExpectedChecksum: 111625,
    MaxMetrics: new() { [Metrics.Gen2] = 10, [Metrics.Retained] = 0, [Metrics.Time] = 14, [Metrics.Alloc] = 90 }), args);
