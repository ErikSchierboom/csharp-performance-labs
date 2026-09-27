using OrderLines;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-02-order-lines",
    Workload: Workload.Run,
    ExpectedChecksum: 29484049425886904,
    MaxMetrics: new() { [Metrics.Time] = 30, [Metrics.Alloc] = 1 }), args);
