using EventHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-01-event-hub: Event hub",
    Workload: Workload.Run,
    ExpectedChecksum: 11995,
    MaxMedianMs: 11,
    MaxAllocatedMb: 49,
    MaxRetainedMb: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
