using OomKilled;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-02-oom-killed",
    Workload: Workload.Run,
    ExpectedChecksum: 240000727200,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Gen2] = 0, [Metrics.Retained] = 1, [Metrics.P99] = 1, [Metrics.Time] = 15, [Metrics.Alloc] = 10 }), args);
