using EventHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-01-event-hub",
    Workload: Workload.Run,
    ExpectedChecksum: 11995,
    MaxMedianMs: 5,
    MaxAllocatedMb: 20,
    MaxRetainedMb: 0,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
