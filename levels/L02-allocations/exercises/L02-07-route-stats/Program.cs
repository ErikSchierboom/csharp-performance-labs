using RouteStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-07-route-stats",
    Workload: Workload.Run,
    ExpectedChecksum: 99457720352575000,
    MaxMedianMs: 20,
    MaxAllocatedMb: 2), args);
