using SyncEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-01-sync-over-async: Sync over async (in an endpoint)",
    Workload: Workload.Run,
    ExpectedChecksum: 160000482831,
    MaxMedianMs: 405,
    MaxAllocatedMb: 7,
    ScaleTime: false,
    MaxP99Ms: 107,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
