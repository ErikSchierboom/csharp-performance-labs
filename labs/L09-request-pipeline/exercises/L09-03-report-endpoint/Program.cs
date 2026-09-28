using ReportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-03-report-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300014235000,
    MaxMetrics: new() { [Metrics.P99] = 3, [Metrics.Time] = 35, [Metrics.Alloc] = 100 }), args);
