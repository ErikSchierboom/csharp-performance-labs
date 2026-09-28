using OrdersService;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-boss-orders-service",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 3, [Metrics.P99] = 40, [Metrics.Time] = 350, [Metrics.Alloc] = 35 }), args);
