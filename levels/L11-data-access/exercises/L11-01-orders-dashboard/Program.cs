using OrdersDashboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-01-orders-dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMedianMs: 55,
    MaxAllocatedMb: 62,
    MaxP99Ms: 7,
    MaxMetrics: new() { ["sqlCommands"] = 601 },
    MeasuredRuns: 9), args);
