using Throttled;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-07-throttled-too-tight: Throttled too tight (concurrency too low)",
    Workload: Workload.Run,
    ExpectedChecksum: 39800,
    MaxMedianMs: 288,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
