using SessionCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-02-session-cache",
    Workload: Workload.Run,
    ExpectedChecksum: 81102788,
    MaxMedianMs: 10,
    MaxAllocatedMb: 202,
    MaxRetainedMb: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
