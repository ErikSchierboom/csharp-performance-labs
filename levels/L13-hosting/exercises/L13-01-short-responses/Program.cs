using ShortResponses;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-01-short-responses",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMedianMs: 19,
    MaxAllocatedMb: 8,
    MaxP99Ms: 4,
    MaxMetrics: new() { [Metrics.Connections] = 25 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
