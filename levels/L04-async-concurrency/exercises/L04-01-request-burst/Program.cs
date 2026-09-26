using RequestBurst;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-01-request-burst",
    Workload: Workload.Run,
    ExpectedChecksum: 60100,
    MaxMedianMs: 25,
    MaxAllocatedMb: 1,
    ScaleTime: false,
    MaxP99Ms: 25,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
