using OrdersDashboard;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-01-orders-dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMedianMs: 35,
    MaxAllocatedMb: 30,
    MaxP99Ms: 5,
    MaxMetrics: new() { [DbMetrics.CommandsPerRequest] = 400 }), args);
