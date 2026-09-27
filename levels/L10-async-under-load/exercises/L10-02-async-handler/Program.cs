using AsyncHandler;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-02-async-handler",
    Workload: Workload.Run,
    ExpectedChecksum: 160000482645,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.P99] = 20, [Metrics.Time] = 100, [Metrics.Alloc] = 3 }), args);
