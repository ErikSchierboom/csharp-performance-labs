using PagedCatalogue;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-04-paged-catalogue",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    MaxMedianMs: 96,
    MaxAllocatedMb: 166,
    MaxRetainedMb: 1,
    MaxP99Ms: 5,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
