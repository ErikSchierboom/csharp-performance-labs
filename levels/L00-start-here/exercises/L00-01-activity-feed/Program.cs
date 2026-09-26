using ActivityFeed;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L00-01-activity-feed",
    Workload: Workload.Run,
    ExpectedChecksum: 576201561659499,
    MaxMedianMs: 29,
    MaxAllocatedMb: 8), args);
