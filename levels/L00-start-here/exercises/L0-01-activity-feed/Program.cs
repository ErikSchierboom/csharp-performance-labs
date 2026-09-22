using ActivityFeed;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L0-01 Activity feed (worked example)",
    Workload: Workload.Run,
    ExpectedChecksum: 576201561659499,
    MaxMedianMs: 50,
    MaxAllocatedMb: 8), args);
