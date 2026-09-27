using BackgroundJobs;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-06-background-jobs",
    Workload: Workload.Run,
    ExpectedChecksum: 60600181800,
    MaxMedianMs: 1000,
    MaxAllocatedMb: 3,
    ScaleTime: false,
    MaxP99Ms: 5,
    MaxMetrics: new() { [Metrics.PeakJobs] = 5 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
