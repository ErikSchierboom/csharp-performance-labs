using SyncOverAsync;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-01-sync-over-async: Sync over async",
    Workload: Workload.Run,
    ExpectedChecksum: 60100,
    MaxMedianMs: 83,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    MaxP99Ms: 82,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
