using EndpointNPlusOne;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-01-endpoint-n-plus-one: N+1 seen from the endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMedianMs: 94,
    MaxAllocatedMb: 62,
    MaxP99Ms: 12,
    MaxMetrics: new() { ["sqlCommands"] = 601 },
    MeasuredRuns: 9), args);
