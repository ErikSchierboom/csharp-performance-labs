using OrdersService;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-boss-orders-service",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMedianMs: 350,
    MaxAllocatedMb: 35,
    MaxP99Ms: 40,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 3 }), args);
