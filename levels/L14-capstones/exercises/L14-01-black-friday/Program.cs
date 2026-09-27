using BlackFriday;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-01-black-friday",
    Workload: Workload.Run,
    ExpectedChecksum: 120000361200,
    MaxMedianMs: 150,
    MaxAllocatedMb: 2.3,
    ScaleTime: false,
    MaxP99Ms: 100,
    MaxMetrics: new() { [Metrics.ExternalCalls] = 46, [Metrics.GiveUps] = 0 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
