using BackgroundJobs;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-06-background-jobs",
    Workload: Workload.Run,
    ExpectedChecksum: 60600181800,
    MaxMedianMs: 1439,
    MaxAllocatedMb: 3,
    ScaleTime: false,
    MaxP99Ms: 7,
    MaxMetrics: new() { ["peakConcurrentJobs"] = 7 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
