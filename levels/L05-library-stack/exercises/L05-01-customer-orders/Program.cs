using CustomerOrders;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L05-01-customer-orders",
    Workload: Workload.Run,
    ExpectedChecksum: 20776276,
    MaxMedianMs: 2,
    MaxAllocatedMb: 1,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 1 }), args);
