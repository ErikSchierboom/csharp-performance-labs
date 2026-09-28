using OrderReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-boss-order-report",
    Workload: Workload.Run,
    ExpectedChecksum: 1240228,
    MaxMetrics: new() { [Metrics.Time] = 2, [Metrics.Alloc] = 1 }), args);
