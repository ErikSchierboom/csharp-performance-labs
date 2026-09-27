using ReportService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-04-report-service",
    Workload: Workload.Run,
    ExpectedChecksum: 240000730200,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 10, [Metrics.Alloc] = 6 }), args);
