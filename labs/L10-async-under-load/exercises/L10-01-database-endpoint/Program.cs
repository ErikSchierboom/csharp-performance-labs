using DatabaseEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-01-database-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 160000482831,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.P99] = 107, [Metrics.Time] = 405, [Metrics.Alloc] = 3 }), args);
