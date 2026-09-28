using FlakyDownstream;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-05-flaky-downstream",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    ScaleTime: false,
    MaxMetrics: new() { [Metrics.DownstreamCalls] = 400, [Metrics.GiveUps] = 0, [Metrics.P99] = 200, [Metrics.Time] = 1000, [Metrics.Alloc] = 2 }), args);
