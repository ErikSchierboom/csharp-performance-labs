using SessionCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-02-session-cache",
    Workload: Workload.Run,
    ExpectedChecksum: 81102788,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 3, [Metrics.Time] = 10, [Metrics.Alloc] = 202 }), args);
