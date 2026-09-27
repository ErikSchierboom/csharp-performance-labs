using FlakyDownstream;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-05-flaky-downstream",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    MaxMedianMs: 1000,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    MaxP99Ms: 200,
    MaxMetrics: new() { [Metrics.DownstreamCalls] = 400, [Metrics.GiveUps] = 0 }), args);
