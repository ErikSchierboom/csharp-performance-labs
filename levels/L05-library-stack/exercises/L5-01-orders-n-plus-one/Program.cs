using OrdersNPlusOne;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-01-orders-n-plus-one: Orders (N+1)",
    Workload: Workload.Run,
    ExpectedChecksum: 20776276,
    MaxMedianMs: 6,
    MaxAllocatedMb: 2,
    MaxMetrics: new() { ["sqlCommands"] = 3 }), args);
