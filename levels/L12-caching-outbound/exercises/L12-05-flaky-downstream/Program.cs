using FlakyDownstream;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-05-flaky-downstream",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    MaxMedianMs: 3246,
    MaxAllocatedMb: 4,
    ScaleTime: false,
    MaxP99Ms: 724,
    MaxMetrics: new() { ["downstreamCalls"] = 601, ["giveUps"] = 1 }), args);
