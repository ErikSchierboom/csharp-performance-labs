using RequestBurst;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-01-request-burst",
    Workload: Workload.Run,
    ExpectedChecksum: 60100,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.P99] = 25, [Metrics.Time] = 25, [Metrics.Alloc] = 1 }), args);
