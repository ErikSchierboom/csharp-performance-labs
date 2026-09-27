using ShortResponses;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-01-short-responses",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Connections] = 25, [Metrics.P99] = 1, [Metrics.Time] = 19, [Metrics.Alloc] = 3 }), args);
