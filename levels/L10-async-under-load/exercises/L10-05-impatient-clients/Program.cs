using ImpatientClients;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-05-impatient-clients",
    Workload: Workload.Run,
    ExpectedChecksum: 4925015475,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.StepsAfterAbort] = 0, [Metrics.PeakInFlight] = 25, [Metrics.P99] = 1500, [Metrics.Time] = 2000, [Metrics.Alloc] = 8 }), args);
