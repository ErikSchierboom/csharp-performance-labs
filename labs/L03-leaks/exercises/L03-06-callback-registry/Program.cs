using CallbackRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-06-callback-registry",
    Workload: Workload.Run,
    ExpectedChecksum: 375876,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 1, [Metrics.Time] = 8, [Metrics.Alloc] = 58 }), args);
