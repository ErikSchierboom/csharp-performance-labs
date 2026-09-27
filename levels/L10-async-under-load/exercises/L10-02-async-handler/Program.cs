using AsyncHandler;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-02-async-handler",
    Workload: Workload.Run,
    ExpectedChecksum: 160000482645,
    MaxMedianMs: 100,
    MaxAllocatedMb: 3,
    ScaleTime: false,
    MaxP99Ms: 20,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
