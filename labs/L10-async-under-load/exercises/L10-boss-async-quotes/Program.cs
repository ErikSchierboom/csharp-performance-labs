using AsyncQuotes;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-boss-async-quotes",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241892,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.PeakInFlight] = 61, [Metrics.P99] = 200, [Metrics.Time] = 700, [Metrics.Alloc] = 5 }), args);
