using ClientLifetime;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-01-client-lifetime: HttpClient created per request",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241600,
    MaxMedianMs: 24,
    MaxAllocatedMb: 6,
    ScaleTime: false,
    MaxP99Ms: 6,
    MaxMetrics: new() { ["connections"] = 25 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
