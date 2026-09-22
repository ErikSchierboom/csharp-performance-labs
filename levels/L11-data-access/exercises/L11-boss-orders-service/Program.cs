using OrdersService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-boss-orders-service: Orders service (final boss of Level 11)",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMedianMs: 1146,
    MaxAllocatedMb: 73,
    ScaleTime: false,
    MaxP99Ms: 108,
    MaxMetrics: new() { ["sqlCommands"] = 1801 }), args);
