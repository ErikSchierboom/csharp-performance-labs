using RequestLogging;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-02-request-logging",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905445,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 15, [Metrics.Alloc] = 10 }), args);
