using AggregationEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-03-aggregation-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 32000096921,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.PeakInFlight] = 40, [Metrics.P99] = 100, [Metrics.Time] = 700, [Metrics.Alloc] = 7 }), args);
