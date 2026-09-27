using PartnerApiCalls;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-07-partner-api-calls",
    Workload: Workload.Run,
    ExpectedChecksum: 39800,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Time] = 80, [Metrics.Alloc] = 1 }), args);
