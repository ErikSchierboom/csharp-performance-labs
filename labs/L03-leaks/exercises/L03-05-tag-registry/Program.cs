using TagRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-05-tag-registry",
    Workload: Workload.Run,
    ExpectedChecksum: 1408890,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 0, [Metrics.Time] = 10, [Metrics.Alloc] = 90 }), args);
