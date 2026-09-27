using CurrencyConversion;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-04-currency-conversion",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 5, [Metrics.Alloc] = 2 }), args);
