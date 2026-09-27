using HelloEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-01-hello-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 800002476000,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 30, [Metrics.Alloc] = 12 }), args);
