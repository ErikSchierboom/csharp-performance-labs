using EventHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-01-event-hub",
    Workload: Workload.Run,
    ExpectedChecksum: 11995,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 0, [Metrics.Time] = 5, [Metrics.Alloc] = 20 }), args);
