using DownstreamCall;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-01-downstream-call",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241600,
    MaxMedianMs: 8,
    MaxAllocatedMb: 3,
    MaxP99Ms: 1,
    MaxMetrics: new() { [Metrics.Connections] = 16 },
    Reset: Workload.Reset), args);
