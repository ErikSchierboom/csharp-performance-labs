using BlackFriday;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-01-black-friday",
    Workload: Workload.Run,
    ExpectedChecksum: 120000361200,
    MaxMedianMs: 245,
    MaxAllocatedMb: 5,
    ScaleTime: false,
    MaxP99Ms: 235,
    MaxMetrics: new() { [Metrics.ExternalCalls] = 46, [Metrics.GiveUps] = 1 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
