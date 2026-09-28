using SessionGateway;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-boss-session-gateway",
    Workload: Workload.Run,
    ExpectedChecksum: 198990,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 1, [Metrics.Time] = 4, [Metrics.Alloc] = 24 }), args);
