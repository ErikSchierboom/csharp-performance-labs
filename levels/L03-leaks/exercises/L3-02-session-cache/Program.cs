using SessionCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-02-session-cache: Session cache",
    Workload: Workload.Run,
    ExpectedChecksum: 81102788,
    MaxMedianMs: 28,
    MaxAllocatedMb: 202,
    MaxRetainedMb: 6,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
