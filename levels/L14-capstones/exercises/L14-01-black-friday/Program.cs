using BlackFriday;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-01-black-friday",
    Workload: Workload.Run,
    ExpectedChecksum: 120000361200,
    ScaleTime: false,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.ExternalCalls] = 46, [Metrics.GiveUps] = 0, [Metrics.P99] = 100, [Metrics.Time] = 150, [Metrics.Alloc] = 2.3 }), args);
