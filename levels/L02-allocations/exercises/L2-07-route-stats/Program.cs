using RouteStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-07 Route stats",
    Workload: Workload.Run,
    ExpectedChecksum: 99457720352575000,
    MaxMedianMs: 60,
    MaxAllocatedMb: 8), args);
