using CustomerOrders;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L05-01-customer-orders",
    Workload: Workload.Run,
    ExpectedChecksum: 20776276,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 1, [Metrics.Time] = 2, [Metrics.Alloc] = 1 }), args);
