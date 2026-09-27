using AggregationEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-03-aggregation-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 32000096921,
    MaxMedianMs: 700,
    MaxAllocatedMb: 7,
    ScaleTime: false,
    MaxP99Ms: 100,
    MaxMetrics: new() { [Metrics.PeakInFlight] = 40 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
