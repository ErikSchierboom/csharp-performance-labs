using BackgroundJobs;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-06-background-jobs",
    Workload: Workload.Run,
    ExpectedChecksum: 60600181800,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.PeakJobs] = 5, [Metrics.P99] = 5, [Metrics.Time] = 1000, [Metrics.Alloc] = 3 }), args);
