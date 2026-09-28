using OrdersDashboard;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-01-orders-dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 400, [Metrics.P99] = 5, [Metrics.Time] = 35, [Metrics.Alloc] = 30 }), args);
