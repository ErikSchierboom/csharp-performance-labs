using CustomerDashboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-05-customer-dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 28465392998,
    MaxMetrics: new() { [Metrics.Time] = 50, [Metrics.Alloc] = 5 }), args);
