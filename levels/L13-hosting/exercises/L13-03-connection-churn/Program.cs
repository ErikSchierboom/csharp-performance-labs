using ConnectionChurn;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-03-connection-churn: Connection churn (`Connection: close`)",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMedianMs: 33,
    MaxAllocatedMb: 8,
    MaxP99Ms: 6,
    MaxMetrics: new() { ["connections"] = 25 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
