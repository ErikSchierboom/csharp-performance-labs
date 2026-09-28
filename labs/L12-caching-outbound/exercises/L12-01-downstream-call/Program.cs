using DownstreamCall;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-01-downstream-call",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241600,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.Connections] = 16, [Metrics.P99] = 1, [Metrics.Time] = 8, [Metrics.Alloc] = 3 }), args);
