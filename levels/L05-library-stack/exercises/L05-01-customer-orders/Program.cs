using CustomerOrders;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-01-customer-orders",
    Workload: Workload.Run,
    ExpectedChecksum: 20776276,
    MaxMedianMs: 2,
    MaxAllocatedMb: 1,
    MaxMetrics: new() { ["sqlCommands"] = 1 }), args);
