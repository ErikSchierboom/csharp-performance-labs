using ImportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-05-import-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMetrics: new() { [Metrics.Gen2] = 0, [Metrics.P99] = 12, [Metrics.Time] = 300, [Metrics.Alloc] = 170 }), args);
