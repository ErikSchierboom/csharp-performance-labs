using PagedCatalogue;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-04-paged-catalogue",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    MaxMedianMs: 60,
    MaxAllocatedMb: 70,
    MaxRetainedMb: 0,
    MaxP99Ms: 5,
    Reset: Workload.Reset), args);
